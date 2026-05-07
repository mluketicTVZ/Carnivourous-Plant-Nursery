---
name: edit-form-page-skill
description: Use when creating or modifying a Create or Edit form page in the Carnivorous Plant Nursery project — building the GET and POST controller actions, writing the repository Add/Update methods, and scaffolding the Razor view with a themed form, validation, and submit/cancel buttons. Keywords: create, edit, form, POST, GET, save, update, add, HttpPost, HttpGet, asp-for, validation, fieldset, entity-form-wrapper, details-section.
---

# Edit / Create Form Page Skill — Carnivorous Plant Nursery

## Skill activation log

**Every time this skill is loaded**, append a single line to `agent_corner/skill_logs/edit-form-page-skill.txt` (relative to the solution root `c:\Brutal Stuff\Git\repo\mluketicTVZ\Carnivourous-Plant-Nursery\`) using the following PowerShell command before doing any other work:

```powershell
"$(Get-Date -Format 'yyyy-MM-ddTHH:mm:ss') - edit-form-page-skill loaded for: <brief task description>" | Add-Content -Path "agent_corner\skill_logs\edit-form-page-skill.txt"
```

Replace `<brief task description>` with a one-line summary of the form being created or modified.

---

## When to use this skill

Use this skill whenever you need to:
- Add `Create` (GET + POST) or `Edit` (GET + POST) actions to a controller
- Add `Add()` or `Update()` methods to a repository
- Scaffold a new `Views/Xyz/Create.cshtml` or `Views/Xyz/Edit.cshtml` form view
- Modify validation rules or field layout on an existing form view

---

## Project identity

- **Solution root**: `c:\Brutal Stuff\Git\repo\mluketicTVZ\Carnivourous-Plant-Nursery\`
- **Project folder**: `Carnivorous-Plant-Nursery\`
- **Root namespace**: `Carnivorous_Plant_Nursery`
- **Target framework**: .NET 8 / ASP.NET Core 8 MVC
- **View engine**: Razor (`.cshtml`) — `_ViewImports.cshtml` already imports tag helpers globally

---

## Controller conventions

- Add `Create` and `Edit` actions alongside the existing `Index`/`Details` actions on the controller
- `[HttpGet]` `Create()` — returns an empty form view (no model parameter needed; populate any select-list `ViewBag`/`ViewData` here)
- `[HttpPost]` `Create(XyzModel model)` — validate with `ModelState.IsValid`, call repository, redirect to `Index` on success
- `[HttpGet]` `Edit(int id)` — load entity from repository, return `NotFound()` if missing, pass entity to view
- `[HttpPost]` `Edit(int id, XyzModel model)` — validate, call repository update, redirect to `Details` on success
- Use `[ValidateAntiForgeryToken]` on every `[HttpPost]` action
- Populate FK select lists in `ViewData` before returning the view (both GET and POST-fail paths)

```csharp
[Route("create")]
[HttpGet]
public IActionResult Create()
{
    ViewData["TaxonomyList"] = _taxonomyRepository.GetAll();
    return View();
}

[Route("create")]
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Plant model)
{
    if (ModelState.IsValid)
    {
        _plantRepository.Add(model);
        return RedirectToAction(nameof(Index));
    }
    ViewData["TaxonomyList"] = _taxonomyRepository.GetAll();
    return View(model);
}

[Route("edit/{id:int}")]
[HttpGet]
public IActionResult Edit(int id)
{
    var entity = _plantRepository.GetById(id);
    if (entity == null) return NotFound();
    ViewData["TaxonomyList"] = _taxonomyRepository.GetAll();
    return View(entity);
}

[Route("edit/{id:int}")]
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, Plant model)
{
    if (ModelState.IsValid)
    {
        _plantRepository.Update(model);
        return RedirectToAction(nameof(Details), new { id });
    }
    ViewData["TaxonomyList"] = _taxonomyRepository.GetAll();
    return View(model);
}
```

---

## Repository conventions

- `Add(T entity)` — `_db.Xyz.Add(entity); _db.SaveChanges();`
- `Update(T entity)` — `_db.Xyz.Update(entity); _db.SaveChanges();`
- Both methods return `void` (or the saved entity if the caller needs the generated Id)
- For FK select lists, add a simple `GetAll()` that returns `List<T>` without heavy includes

```csharp
public void Add(Xyz entity)
{
    _db.Xyz.Add(entity);
    _db.SaveChanges();
}

public void Update(Xyz entity)
{
    _db.Xyz.Update(entity);
    _db.SaveChanges();
}
```

---

## View structure — `Views/Xyz/Create.cshtml` / `Edit.cshtml`

The project uses a **bespoke design language** — do **not** use default Bootstrap `.container`, `.row`, `.col`, or `.form-group` classes. Mirror the visual structure of the existing `Details.cshtml` views.

### Page skeleton

```html
@model Carnivorous_Plant_Nursery.Models.Xyz

@{
    ViewData["Title"] = "Create Xyz";   // or "Edit Xyz"
}

<!-- Breadcrumb -->
<div class="nursery-breadcrumb">
    <a asp-controller="Home" asp-action="Index">Home</a> &rsaquo;
    <a asp-action="Index">Xyz</a> &rsaquo; Create
</div>

<partial name="_BackButton" />

<h1 style="color: var(--dark-green); border-bottom: 3px solid var(--accent-color); padding-bottom: 8px;">
    Create Xyz   <!-- or Edit Xyz -->
</h1>

<div class="entity-form-wrapper">
    <form asp-action="Create" method="post">   <!-- asp-action="Edit" for edit form -->
        @Html.AntiForgeryToken()

        <!-- Group related fields into fieldsets that match the sections in Details views -->
        <fieldset class="details-section">
            <legend>Basic Information</legend>

            <div class="filter-group">
                <label asp-for="PropertyName" class="card-label"></label>
                <input asp-for="PropertyName" />
                <span asp-validation-for="PropertyName" class="val-error"></span>
            </div>

            <!-- Repeat filter-group per field -->
        </fieldset>

        <fieldset class="details-section">
            <legend>Additional Details</legend>
            <!-- More fields -->
        </fieldset>

        <!-- Submit / Cancel -->
        <div style="display: flex; gap: 10px; margin-top: 20px;">
            <button type="submit" class="btn-leaf">Save</button>   <!-- use btn-inventory for logistical entities -->
            <a asp-action="Index" class="btn-clear">Cancel</a>
        </div>
    </form>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### Thematic class guide

| Domain | Submit button | Wrapper | Form sections |
|---|---|---|---|
| Botanical (Plant, SeedBatch, CareProfile) | `btn-leaf` | `entity-form-wrapper` | `details-section` |
| Logistical (Inventory) | `btn-inventory` | `entity-form-wrapper` | `details-section` |

### CSS variables in use
- `var(--dark-green)` — headings, borders
- `var(--accent-color)` — decorative borders
- `.val-error` — validation error messages (do **not** use Bootstrap `.text-danger`)

---

## Input type mapping per field type

| Model type | HTML input | Notes |
|---|---|---|
| `string` | `<input asp-for="..." />` | Renders as `type="text"` |
| `string` (long description) | `<textarea asp-for="..."></textarea>` | For fields annotated `[MaxLength(500)]` or larger |
| `int` / `int?` | `<input asp-for="..." type="number" />` | Add `min="0"` if non-negative constraint exists |
| `decimal` / `decimal?` | `<input asp-for="..." type="number" step="0.01" />` | Add `min="0"` if non-negative |
| `bool` / `bool?` | `<input asp-for="..." type="checkbox" />` | Wrap in `.filter-group-checkbox` |
| `DateTime` / `DateTime?` | `<input asp-for="..." type="date" />` | — |
| Enum | `<select asp-for="..." asp-items="Html.GetEnumSelectList<TEnum>()">` | Add `<option value="">-- Select --</option>` for nullable enums |
| FK int (e.g. `TaxonomyId`) | `<select asp-for="TaxonomyId">` populated from `ViewData` | See FK select pattern below |

### Enum select pattern

```html
<div class="filter-group">
    <label asp-for="CurrentStage" class="card-label"></label>
    <select asp-for="CurrentStage" asp-items="Html.GetEnumSelectList<PlantStage>()">
        <option value="">-- Select Stage --</option>
    </select>
    <span asp-validation-for="CurrentStage" class="val-error"></span>
</div>
```

### FK select pattern (e.g. TaxonomyId)

In the controller, populate `ViewData["TaxonomyList"]` with `List<Taxonomy>` before returning the view. Then in the view:

```html
<div class="filter-group">
    <label asp-for="TaxonomyId" class="card-label"></label>
    <select asp-for="TaxonomyId">
        <option value="">-- Select Taxonomy --</option>
        @foreach (var t in (List<Taxonomy>)ViewData["TaxonomyList"]!)
        {
            <option value="@t.Id" @(Model?.TaxonomyId == t.Id ? "selected" : "")>@t.FullName</option>
        }
    </select>
    <span asp-validation-for="TaxonomyId" class="val-error"></span>
</div>
```

---

## Field groupings per entity

Use these to decide how to split `<fieldset>` sections in the form view.

### Plant
| Fieldset | Fields |
|---|---|
| Listing | SKU, ListingTitle, Price, Description, IsAvailableInWebshop |
| Acquisition | DateAcquired, InternalNotes, LocationInNursery, TaxonomyId, LineageId |
| Botanical | CurrentStage, PotDiameterCm, PotHeightCm, LastRepottingDate, LastDormancyDateStart, LastDormancyDateEnd, EstimatedAgeAtAcquiryYears |
| Health | HealthStatus, HealthDescription |

### SeedBatch
| Fieldset | Fields |
|---|---|
| Listing | SKU, ListingTitle, Price, Description, IsAvailableInWebshop |
| Acquisition | DateAcquired, InternalNotes, LocationInNursery, TaxonomyId, LineageId |
| Seed Metadata | SeedCount, HarvestDate, RequiresStratification, ExpectedViabilityMonths, EstimatedGerminationRate |

### CareProfile
| Fieldset | Fields |
|---|---|
| Identity | CareProfileName |
| Light & Humidity | RequiredLight, RequiredHumidity |
| Temperature | MinTemperature, MaxTemperature, TemperatureDescription, RequiresWinterDormancy |
| Soil & Care | SoilMix, CareDescription |

### Taxonomy
| Fieldset | Fields |
|---|---|
| Classification | Genus, Species, Cultivar, CommonName |
| Care | CareProfileId |

### Lineage
| Fieldset | Fields |
|---|---|
| Parents | MotherId, FatherId |
| Genetics | Generation, IsClone, GeneticsDescription |

---

## Step-by-step checklist

1. **Repository** — add `Add(T entity)` and `Update(T entity)` methods; add `GetAll()` on any referenced FK repository
2. **Controller** — add `Create` (GET + POST) and/or `Edit` (GET + POST) actions; add `[ValidateAntiForgeryToken]` to POST; populate `ViewData` FK lists on both GET and POST-fail paths
3. **View** — create `Views/Xyz/Create.cshtml` and/or `Edit.cshtml`; follow the skeleton above; use fieldset groupings from the table above; choose botanical or logistical theme
4. **Validation scripts** — always include `@section Scripts { <partial name="_ValidationScriptsPartial" /> }` at the bottom of the view
5. **Navigation** — add a "Create New" link to the corresponding `Index.cshtml` (use the same themed button class)
6. **Sitemap** — update `docs/sitemap.md` with the new URL entries
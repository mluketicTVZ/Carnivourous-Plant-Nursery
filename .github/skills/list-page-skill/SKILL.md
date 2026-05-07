---
name: list-page-skill
description: Use when creating or modifying a list/index page in the Carnivorous Plant Nursery project — building the controller Index action, writing the repository GetAll/Search query, and scaffolding the Razor view with search filters and entity cards. Keywords: list, index, catalog, search, filter, card, nursery-card, inventory-card, Index action, GetAll, Search, IActionResult.
---

# List Page Skill — Carnivorous Plant Nursery

## Skill activation log

**Every time this skill is loaded**, append a single line to `agent_corner/skill_logs/list-page-skill.txt` (relative to the solution root `c:\Brutal Stuff\Git\repo\mluketicTVZ\Carnivourous-Plant-Nursery\`) using the following PowerShell command before doing any other work:

```powershell
"$(Get-Date -Format 'yyyy-MM-ddTHH:mm:ss') - list-page-skill loaded for: <brief task description>" | Add-Content -Path "agent_corner\skill_logs\list-page-skill.txt"
```

Replace `<brief task description>` with a one-line summary of the list page being created or modified.

---

## When to use this skill

Use this skill whenever you need to:
- Create a new `Index` action on an existing or new controller
- Add a `GetAll()` or `Search()` method to a repository
- Scaffold a new `Views/Xxx/Index.cshtml` list page
- Add or change search/filter parameters on an existing list page
- Modify how entity cards are rendered on any list view

---

## Project identity

- **Solution root**: `c:\Brutal Stuff\Git\repo\mluketicTVZ\Carnivourous-Plant-Nursery\`
- **Project folder**: `Carnivorous-Plant-Nursery\`
- **Root namespace**: `Carnivorous_Plant_Nursery`
- **Target framework**: .NET 8 / ASP.NET Core 8 MVC
- **View engine**: Razor (`.cshtml`) — `_ViewImports.cshtml` already imports tag helpers globally

---

## Controller conventions

- All controllers live in `Carnivorous-Plant-Nursery/Controllers/`
- Class is decorated with `[Route("slug")]` at the class level (e.g. `[Route("plants")]`)
- The `Index` action is decorated with `[Route("")]`
- Query-string filter parameters are accepted as optional action parameters with defaults (e.g. `string? searchTerm`, `bool webshopOnly = false`, enum nullable params)
- Pass filter values back to the view via `ViewData["SearchTerm"] = searchTerm;` etc. so the filter form can preserve its state
- Inject the repository via constructor DI

```csharp
[Route("xyz")]
public class XyzController : Controller
{
    private readonly XyzRepository _xyzRepository;

    public XyzController(XyzRepository xyzRepository)
    {
        _xyzRepository = xyzRepository;
    }

    [Route("")]
    public IActionResult Index(string? searchTerm, bool webshopOnly = false)
    {
        var items = _xyzRepository.Search(searchTerm ?? string.Empty, webshopOnly);
        ViewData["SearchTerm"] = searchTerm;
        ViewData["WebshopOnly"] = webshopOnly;
        return View(items);
    }
}
```

---

## Repository conventions

- All repositories live in `Carnivorous-Plant-Nursery/Repositories/`
- Constructor accepts `AppDbContext db` and stores it as `_db`
- `Search` method signature: `public List<Xyz> Search(string searchTerm, ...)` — always filter in SQL (`.Where(...)` before `.ToList()`)
- Use `.Include(...)` for every navigation property the view will display
- If filtering on a computed C# property (e.g. `FullName`), call `.ToList()` first, then filter in C#
- Register the new repository as `AddScoped` in `Program.cs` if it is new

```csharp
public class XyzRepository
{
    private readonly AppDbContext _db;

    public XyzRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Xyz> Search(string searchTerm)
    {
        return _db.Xyz
            .Include(x => x.RelatedEntity)
            .Where(x => string.IsNullOrEmpty(searchTerm) || x.Name.Contains(searchTerm))
            .ToList();
    }
}
```

---

## View structure — `Views/Xyz/Index.cshtml`

The project uses a **bespoke design language** — do **not** use default Bootstrap `.container`, `.row`, `.col`, or `.card` classes. Follow the patterns below exactly.

### Page skeleton

```html
@model List<Carnivorous_Plant_Nursery.Models.Xyz>

@{
    ViewData["Title"] = "Xyz Catalog";
}

<!-- Breadcrumb -->
<div class="nursery-breadcrumb">
    <a asp-controller="Home" asp-action="Index">Home</a> &rsaquo; Xyz
</div>

<!-- Page header -->
<h1 style="color: var(--dark-green); border-bottom: 3px solid var(--accent-color); padding-bottom: 8px;">
    Xyz Catalog
</h1>
<p>Brief description of what this page shows.</p>

<!-- Search / filter form -->
<form method="get" class="botanical-filter-form">   <!-- use inventory-filter-form for logistical entities -->
    <div class="filter-group">
        <label for="searchTerm">Search</label>
        <input type="text" id="searchTerm" name="searchTerm" value="@ViewData["SearchTerm"]" placeholder="Search..." />
    </div>
    <!-- Add more filter-group divs for additional filters (enums, booleans, etc.) -->
    <div style="display: flex; gap: 10px;">
        <button type="submit" class="btn-filter">Filter</button>   <!-- use btn-filter-inventory for logistical entities -->
        <a asp-action="Index" class="btn-clear">Clear</a>
    </div>
</form>

<!-- Entity list -->
@if (Model != null && Model.Any())
{
    <div class="entity-catalog">
        @foreach (var item in Model)
        {
            <div class="nursery-card">   <!-- use inventory-card for logistical entities -->
                <div class="nursery-card-header">
                    <h2 class="nursery-card-title">@item.PrimaryDisplayField</h2>
                    <span>@item.SubtitleField</span>
                </div>
                <div class="nursery-card-body">
                    <p><span class="card-label">Field label:</span> @item.SomeField</p>
                    <!-- Status badges use: <span class="status-badge status-instock">In Stock</span> -->
                </div>
                <div class="nursery-card-footer">
                    <a asp-action="Details" asp-route-id="@item.Id" class="btn-leaf">View Details</a>
                    <!-- use btn-inventory for logistical entities -->
                </div>
            </div>
        }
    </div>
}
else
{
    <p>No items found.</p>
}
```

### Thematic class guide

| Domain | Card class | Button class | Filter form class | Filter button class |
|---|---|---|---|---|
| Botanical (Plant, SeedBatch, CareProfile) | `nursery-card` | `btn-leaf` | `botanical-filter-form` | `btn-filter` |
| Logistical (Inventory) | `inventory-card` | `btn-inventory` | `inventory-filter-form` | `btn-filter-inventory` |

### CSS variables in use
- `var(--dark-green)` — headings, borders
- `var(--accent-color)` — decorative borders, highlights
- `var(--primary-green)` — interactive elements on botanical views

### Enum filter selects

For enum filter parameters, generate the select using `@Html.GetEnumSelectList<YourEnum>()`:

```html
<div class="filter-group">
    <label for="stage">Stage</label>
    <select id="stage" name="stage">
        <option value="">All</option>
        @foreach (var opt in Html.GetEnumSelectList<PlantStage>())
        {
            <option value="@opt.Value" @(ViewData["Stage"]?.ToString() == opt.Value ? "selected" : "")>@opt.Text</option>
        }
    </select>
</div>
```

### Checkbox filter (boolean)

```html
<div class="filter-group filter-group-checkbox">
    <label>
        <input type="checkbox" name="webshopOnly" value="true"
               @(ViewData["WebshopOnly"] as bool? == true ? "checked" : "") />
        Webshop only
    </label>
</div>
```

### Status badge pattern

```html
@{
    var badgeClass = item.IsAvailableInWebshop ? "status-instock" : "status-out";
    var badgeText  = item.IsAvailableInWebshop ? "In Webshop"    : "Not Listed";
}
<span class="status-badge @badgeClass">@badgeText</span>
```

---

## Step-by-step checklist

1. **Repository** — add `Search(...)` or `GetAll()` method; include necessary nav props; register with `AddScoped` in `Program.cs` if new
2. **Controller** — add (or create) controller with `[Route("slug")]`; write `Index` action with filter params; store filter values in `ViewData`
3. **View** — create `Views/Xyz/Index.cshtml`; follow the skeleton above; choose botanical or logistical theme
4. **Navigation** — add a link to the new page in `Views/Shared/_Layout.cshtml` nav section if appropriate
5. **Sitemap** — update `docs/sitemap.md` with the new URL entry
# Codex Guidance - Carnivorous Plant Nursery

This file is the Codex-ready project guidance for the Carnivorous Plant Nursery app. It is based on the previous VS Code agent setup, but it is intentionally self-contained so future work does not need to re-read the old `.github/agents` or `.github/skills` files.

## Project Identity

- Solution root: `C:\Brutal Stuff\Git\repo\mluketicTVZ\Carnivourous-Plant-Nursery`
- ASP.NET project: `Carnivorous-Plant-Nursery\`
- Solution file: `Carnivorous-Plant-Nursery.sln`
- Root namespace: `Carnivorous_Plant_Nursery`
- Target framework: .NET 8 / ASP.NET Core 8 MVC
- View engine: Razor `.cshtml`; `_ViewImports.cshtml` imports tag helpers globally
- Database: SQL Server 2022 in Docker on `localhost,1433`
- Connection string key: `ConnectionStrings:DefaultConnection`

The application manages a carnivorous plant nursery inventory, with a later webshop direction. The core domain includes `Plant`, `SeedBatch`, `InventoryItem`, `Article`, `Taxonomy`, `CareProfile`, and `Lineage`.

## Before Changing Code

- Read the relevant controller, repository, model, view, and docs before editing.
- Preserve the existing MVC + Repository pattern. Keep controllers lean and push data access/query logic into repositories.
- Prefer existing local patterns, CSS classes, shared partials, and naming conventions over new abstractions.
- Do not remove or rewrite unrelated user changes.
- After meaningful route/domain changes, check whether `docs/sitemap.md` and `docs/semantic-model.md` need updates.
- After adding large features or resolving serious failures, consider whether this file needs a short "Known Pitfalls" update.

## Useful Commands

Run from the solution root unless a command clearly needs the project folder.

```powershell
dotnet build Carnivorous-Plant-Nursery.sln
dotnet run --project Carnivorous-Plant-Nursery
dotnet ef migrations add <MigrationName> --project Carnivorous-Plant-Nursery --context AppDbContext
dotnet ef database update --project Carnivorous-Plant-Nursery --context AppDbContext
dotnet ef migrations remove --project Carnivorous-Plant-Nursery --context AppDbContext
dotnet ef migrations script --project Carnivorous-Plant-Nursery --context AppDbContext
```

Do not start Docker, reset databases, run migrations, or apply database updates unless the user asks or the task clearly requires it.

## Backend Standards

- Use modern, clean C# and keep responsibilities separated.
- Use singular filenames for `.cs` and similar code files by default, including DTO, mapper, view-model, controller, repository, and service files. Folders may remain plural when they group multiple files.
- Use constructor DI for repositories and services.
- Register EF repositories with `AddScoped` in `Program.cs`.
- Keep user-facing C# strings out of controllers/repositories:
  - Errors and validation messages go in `Models/ErrorMessage.cs`.
  - Display labels, fallback names, and UI copy used from C# go in `Models/DisplayConstant.cs`.
  - Razor-only presentation text can stay in views.
- For EF-backed methods, use async APIs and return `Task<T>` or `Task`:
  - `SaveChangesAsync`, `ToListAsync`, `FirstOrDefaultAsync`, `FindAsync`, `AnyAsync`, `CountAsync`, etc.
  - Controller actions calling async repositories should be `async Task<IActionResult>`.
- Push filtering to SQL. Avoid mid-query `.ToList()` before `.Where(...)` unless EF cannot translate the expression and the reason is clear.
- Use `.Include(...)` for navigation properties the caller/view needs; avoid accidental N+1 behavior.
- View models should contain exactly what the view needs.

## EF Core Model Rules

- `AppDbContext` lives at `Carnivorous-Plant-Nursery/Data/AppDbContext.cs`.
- Current `DbSet` names are singular: `Plant`, `SeedBatch`, `Taxonomy`, `CareProfile`, `Lineage`.
- `Article` and `InventoryItem` are abstract base classes. There is no direct `DbSet<InventoryItem>` or `DbSet<Article>`.
- `Plant` and `SeedBatch` are inventory subtypes. Query them through their concrete DbSets; `InventoryRepository` combines them where needed.
- Use EF annotations consistently:
  - `[Key]` on `Id`
  - `[Required]` only for DB-required values
  - `[MaxLength(N)]` for strings, commonly 50 for codes, 200 for names/titles, 500-1000 for descriptions/notes
  - `[ForeignKey("NavigationName")]` on FK int properties
  - `[Column(TypeName = "decimal(18,2)")]` on decimals
  - `virtual` navigation properties
  - `ICollection<T>` for one-to-many collections
- Soft delete uses `DeletedAt` on `Article`, `Taxonomy`, and `CareProfile`. List/search/details queries should exclude deleted rows unless intentionally showing archived data.
- Existing relationship pitfall: `Lineage.Mother` and `Lineage.Father` point to `InventoryItem` and use restricted delete behavior.
- Existing relationship pitfall: `Plant.SourceSeedBatchId` uses `OnDelete(DeleteBehavior.NoAction)` to avoid cascade/path problems.

## Routing And MVC

- Controllers live in `Carnivorous-Plant-Nursery/Controllers/`.
- Views live in `Carnivorous-Plant-Nursery/Views/<ControllerNameWithoutController>/`.
- Prefer semantic attribute routes already used by the app:
  - Controller class: `[Route("plants")]`, `[Route("seeds")]`, etc.
  - Index action: `[Route("")]`
  - Details: `[Route("{id:int}")]`
  - Create: `[Route("create")]`
  - Edit: `[Route("edit/{id:int}")]`
  - Delete: `[Route("delete/{id:int}")]`
- POST actions need `[ValidateAntiForgeryToken]`.
- Protected Create/Edit/Delete behavior is Identity role-gated. Use `Admin` or `Manager` for Create/Edit and `Admin` for Delete unless the task defines a different rule.
- When adding or changing URLs, update `docs/sitemap.md`.

## List / Index Page Workflow

Use this when creating or modifying list, catalog, search, filter, or card views.

1. Repository: add or update `Search(...)`/`GetAll...Async(...)`; include nav properties required by the view; filter deleted rows; keep SQL filters before materialization.
2. Controller: add optional query parameters such as `searchTerm`, `webshopOnly`, enum filters; pass selected filter values back through `ViewData`.
3. View: create/update `Views/Xxx/Index.cshtml`; include breadcrumb, filter form, empty state, and cards.
4. Navigation: add a link in `Views/Shared/_Layout.cshtml` only if the page should be globally reachable.
5. Docs: update `docs/sitemap.md`.

Thematic classes:

| Domain | Card | Button | Filter form | Filter button |
|---|---|---|---|---|
| Botanical (`Plant`, `SeedBatch`, `CareProfile`) | `nursery-card` | `btn-leaf` | `botanical-filter-form` | `btn-filter` |
| Logistical (`Inventory`) | `inventory-card` | `btn-inventory` | `inventory-filter-form` | `btn-filter-inventory` |

For enum filters, use `Html.GetEnumSelectList<TEnum>()`. For boolean filters, preserve checkbox state from `ViewData`.

## Create / Edit Form Workflow

Use this when creating or modifying Create/Edit pages.

1. Controller GET: populate FK select data in `ViewData`/`ViewBag`; return the form view.
2. Controller POST: validate `ModelState`; save through repository; redirect to `Details` or `Index` on success; repopulate FK select data before returning invalid forms.
3. Repository: add/update async `AddAsync`, `UpdateAsync`, or focused save methods.
4. View: use `asp-for`, validation spans, anti-forgery token, and themed form sections.
5. Scripts: include `_ValidationScriptsPartial`.
6. Docs: update `docs/sitemap.md` for new routes.

Form styling conventions:

- Use `entity-form-wrapper` around forms.
- Group fields with `fieldset.details-section`.
- Use `.filter-group` for individual controls.
- Use `.val-error` for validation messages, not Bootstrap `.text-danger`.
- Use `_ValidationToast` where existing form views use it.
- Use `_BackButton` where existing views do.

Common field groupings:

- Plant: Listing, Acquisition, Botanical, Health.
- SeedBatch: Listing, Acquisition, Seed Metadata.
- CareProfile: Identity, Light & Humidity, Temperature, Soil & Care.
- Taxonomy: Classification, Care.
- Lineage: Parents, Genetics.

## Frontend And UX

- Do not ship default/plain Bootstrap-looking pages. The app needs a distinct custom nursery design.
- The visual tone should be natural, educational, welcoming, and botanical: earthy tones, botanical greens, crisp whites, and soft sunlight highlights. Avoid a "monster plant" trope.
- Maintain strong contrast, readable spacing, logical layout, and responsive behavior.
- Reuse and extend existing CSS in `wwwroot/css/site.css` and existing shared partials.
- Do not copy/paste repeated UI. Extract near-identical repeated markup into a shared partial in `Views/Shared/`.
- Existing shared partials include:
  - `_ValidationToast`
  - `_BackButton`
  - `_HybridDropdown`
  - `_PlantAutocomplete`
  - `_DatePicker`
- Build complete navigation paths between Index, Details, Create, and Edit pages.
- For frontend changes, run the app and visually verify the affected pages when feasible.

## Documentation Upkeep

- `docs/semantic-model.md` should reflect current domain classes, properties, enums, inheritance, relationships, soft-delete behavior, and important constraints.
- `docs/sitemap.md` should reflect current routes, query filters, controller actions, views, redirects, JSON endpoints, admin/session requirements, and shared partials.
- Documentation does not need to become a changelog. Update it when behavior or structure changes, not for tiny internal refactors.
- `AGENTS.md` should be updated when a new durable rule, known pitfall, command, or workflow would help future agents avoid repeated mistakes.

## Codex Hooks

- Repo-local Codex hooks live in `.codex/hooks.json`.
- The current `UserPromptSubmit` and `PreToolUse` hooks log payloads to `lab-5/agent_log.txt` through `.codex/hooks/log-user-prompt.ps1`.
- Project-local hooks may need to be reviewed/trusted by Codex before they run.
- Keep hooks small and non-blocking unless the user explicitly asks for guardrail behavior.

## Known Pitfalls

- The repository folder name is misspelled as `Carnivourous-Plant-Nursery`, while the ASP.NET project is `Carnivorous-Plant-Nursery`. Be precise with paths.
- The root namespace uses underscores: `Carnivorous_Plant_Nursery`.
- Some older guidance used synchronous repository examples. Prefer async EF operations for current work.
- Do not add `DbSet<InventoryItem>` unless the mapping strategy is intentionally changed.
- Computed properties like `Taxonomy.FullName` may not translate to SQL; filter in SQL on underlying fields where possible, or materialize intentionally with a clear reason.
- Do not hard-delete entities that use `DeletedAt`; preserve soft-delete semantics and filter deleted rows out of normal user-facing queries.
- Delete operations can fail because lineage or source-seed relationships reference the item. Preserve existing `TempData` error behavior and constants.
- `DbSeeder.Seed(db)` runs at startup inside a manual DI scope. Changes to required fields or relationships may need matching seed updates.

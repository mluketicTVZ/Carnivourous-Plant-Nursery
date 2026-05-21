# Sitemap — Carnivorous Plant Nursery Routing Model

## Global Route (Program.cs)

```
{controller=Home}/{action=Index}/{id?}
```

Default: `controller = Home`, `action = Index`

All controllers also use **attribute routing** via `[Route]` on the controller class and individual actions, as detailed below.

---

## HomeController `[Route("home")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/` | GET | HomeController | Index() | Views/Home/Index.cshtml |
| `/home` | GET | HomeController | Index() | Views/Home/Index.cshtml |
| `/home/privacy` | GET | HomeController | Privacy() | Views/Home/Privacy.cshtml |
| `/home/error` | GET | HomeController | Error() | Views/Shared/Error.cshtml |

**Notes**: `Index()` populates `HomeIndexViewModel` (Taxonomies, WebshopItems, SeedsForStratification, LineagePlants).

---

## PlantController `[Route("plants")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/plants` | GET | PlantController | Index() | Views/Plant/Index.cshtml |
| `/plants?searchTerm=X` | GET | PlantController | Index(string searchTerm) | Views/Plant/Index.cshtml |
| `/plants?webshopOnly=true` | GET | PlantController | Index(bool webshopOnly) | Views/Plant/Index.cshtml |
| `/plants?stage=Mature` | GET | PlantController | Index(PlantStage? stage) | Views/Plant/Index.cshtml |
| `/plants?healthStatus=Good` | GET | PlantController | Index(HealthState? healthStatus) | Views/Plant/Index.cshtml |
| `/plants/{id}` | GET | PlantController | Details(int id) | Views/Plant/Details.cshtml |
| `/plants/create` | GET | PlantController | Create() | Views/Plant/Create.cshtml |
| `/plants/create` | POST | PlantController | Create(Plant model) | Redirects to Details on success |
| `/plants/edit/{id}` | GET | PlantController | Edit(int id) | Views/Plant/Edit.cshtml |
| `/plants/edit/{id}` | POST | PlantController | Edit(int id, Plant model) | Redirects to Details on success |
| `/plants/delete/{id}` | POST | PlantController | Delete(int id) | Redirects to Index on success, Details on FK error |
| `/plants/suggestions?term=X` | GET | PlantController | Suggestions(string term) | JSON: `[{text, value}]` |

**Notes**: `Details()` returns `NotFound()` if no plant with the given id exists. Create/Edit/Delete require admin session. Delete sets `TempData["DeleteError"]` and redirects to Details if a lineage entry references this plant. `Suggestions()` returns up to 8 matching results for the AJAX autocomplete control.

---

## InventoryController `[Route("inventory")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/inventory` | GET | InventoryController | Index() | Views/Inventory/Index.cshtml |
| `/inventory?searchTerm=X` | GET | InventoryController | Index(string searchTerm) | Views/Inventory/Index.cshtml |
| `/inventory?webshopOnly=true` | GET | InventoryController | Index(bool webshopOnly) | Views/Inventory/Index.cshtml |
| `/inventory/{id}` | GET | InventoryController | Details(int id) | Views/Inventory/Details.cshtml |
| `/inventory/suggestions?term=X` | GET | InventoryController | Suggestions(string term) | JSON: `[{text, value}]` |

**Notes**: Lists all inventory items (both Plants and SeedBatches). `Details()` returns `NotFound()` if id is not found. `Suggestions()` returns up to 8 matching results for the AJAX autocomplete control.

---

## CareProfileController `[Route("care")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/care` | GET | CareProfileController | Index() | Views/CareProfile/Index.cshtml |
| `/care?searchTerm=X` | GET | CareProfileController | Index(string searchTerm) | Views/CareProfile/Index.cshtml |
| `/care?requiredLight=X` | GET | CareProfileController | Index(string requiredLight) | Views/CareProfile/Index.cshtml |
| `/care/{id}` | GET | CareProfileController | Details(int id) | Views/CareProfile/Details.cshtml |
| `/care/create` | GET | CareProfileController | Create() | Views/CareProfile/Create.cshtml |
| `/care/create` | POST | CareProfileController | Create(CareProfile model) | Redirects to Details on success |
| `/care/edit/{id}` | GET | CareProfileController | Edit(int id) | Views/CareProfile/Edit.cshtml |
| `/care/edit/{id}` | POST | CareProfileController | Edit(int id, CareProfile model) | Redirects to Details on success |
| `/care/delete/{id}` | POST | CareProfileController | Delete(int id) | Redirects to Index on success, Details on FK error |

**Notes**: `Details()` returns `NotFound()` if id is not found. Create/Edit/Delete require admin session. Delete sets `TempData["DeleteError"]` and redirects to Details if a taxonomy references this care profile.

---

## SeedBatchController `[Route("seeds")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/seeds` | GET | SeedBatchController | Index() | Views/SeedBatch/Index.cshtml |
| `/seeds?searchTerm=X` | GET | SeedBatchController | Index(string searchTerm) | Views/SeedBatch/Index.cshtml |
| `/seeds?availableInWebshop=true` | GET | SeedBatchController | Index(bool availableInWebshop) | Views/SeedBatch/Index.cshtml |
| `/seeds/{id}` | GET | SeedBatchController | Details(int id) | Views/SeedBatch/Details.cshtml |
| `/seeds/create` | GET | SeedBatchController | Create() | Views/SeedBatch/Create.cshtml |
| `/seeds/create` | POST | SeedBatchController | Create(SeedBatch model) | Redirects to Details on success |
| `/seeds/edit/{id}` | GET | SeedBatchController | Edit(int id) | Views/SeedBatch/Edit.cshtml |
| `/seeds/edit/{id}` | POST | SeedBatchController | Edit(int id, SeedBatch model) | Redirects to Details on success |
| `/seeds/delete/{id}` | POST | SeedBatchController | Delete(int id) | Redirects to Index on success, Details on FK error |
| `/seeds/suggestions?term=X` | GET | SeedBatchController | Suggestions(string term) | JSON: `[{text, value}]` |

**Notes**: `Details()` returns `NotFound()` if id is not found. Create/Edit/Delete require admin session. Delete sets `TempData["DeleteError"]` and redirects to Details if a lineage entry references this seed batch. `Suggestions()` returns up to 8 matching results for the AJAX autocomplete control.

---

## TaxonomyController `[Route("taxonomy")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/taxonomy` | GET | TaxonomyController | Index() | Views/Taxonomy/Index.cshtml |
| `/taxonomy?searchTerm=X` | GET | TaxonomyController | Index(string searchTerm) | Views/Taxonomy/Index.cshtml |
| `/taxonomy/{id}` | GET | TaxonomyController | Details(int id) | Views/Taxonomy/Details.cshtml |
| `/taxonomy/create` | GET | TaxonomyController | Create() | Views/Taxonomy/Create.cshtml |
| `/taxonomy/create` | POST | TaxonomyController | Create(Taxonomy model) | Redirects to Details on success |
| `/taxonomy/edit/{id}` | GET | TaxonomyController | Edit(int id) | Views/Taxonomy/Edit.cshtml |
| `/taxonomy/edit/{id}` | POST | TaxonomyController | Edit(int id, Taxonomy model) | Redirects to Details on success |
| `/taxonomy/delete/{id}` | POST | TaxonomyController | Delete(int id) | Redirects to Index on success, Details on FK error |
| `/taxonomy/suggestions?term=X` | GET | TaxonomyController | Suggestions(string term) | JSON: `[{text, value}]` |
| `/taxonomy/id-suggestions?term=X` | GET | TaxonomyController | IdSuggestions(string term) | JSON: `[{text, value}]` (value = taxonomy ID) |

**Notes**: `Details()` returns `NotFound()` if id is not found. Create/Edit/Delete require admin session. Delete sets `TempData["DeleteError"]` and redirects to Details if a plant or seed batch references this taxonomy. `Suggestions()` returns display-name matches for the search autocomplete; `IdSuggestions()` returns matches with the numeric ID as value, used by Plant/SeedBatch FK pickers.

---

## AdminController `[Route("admin")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/admin` | GET | AdminController | Index() | Views/Admin/Login.cshtml |
| `/admin` | POST | AdminController | Login(string passkey) | Redirects to `/` on success, re-renders Login on failure |
| `/admin/logout` | POST | AdminController | Logout() | Redirects to `/` |

**Notes**: Login checks `passkey` against `appsettings.json "AdminPasskey"`. On success sets `HttpContext.Session["IsAdmin"] = "true"`. Logout removes that session key. Admin state gates Create/Edit/Delete actions across all entity controllers.

---

## Shared Partial Views

These are rendered inside other views and do not have their own URLs:

| Partial View | Used In | Purpose |
|---|---|---|
| Views/Shared/_Layout.cshtml | All views (via _ViewStart) | Master layout shell |
| Views/Shared/_BackButton.cshtml | Detail/list views | Back navigation button |
| Views/Shared/_ValidationScriptsPartial.cshtml | All form views | jQuery Validate setup with blur triggering |
| Views/Shared/_ValidationToast.cshtml | All Create/Edit form views | Server-side error toast shown on ModelState failure |
| Views/Shared/_PlantAutocomplete.cshtml | Index search forms, Create/Edit FK fields | AJAX autocomplete input (search and select modes) |
| Views/Shared/_HybridDropdown.cshtml | Index filter forms, Create/Edit enum fields | Client-side filtered dropdown for small collections |
| Views/Shared/_DatePicker.cshtml | Plant/SeedBatch Create/Edit forms | Custom date picker with hr/en culture detection |

---

## Summary Table

| Section | Base URL | Actions | Query Filters |
|---|---|---|---|
| Home | `/` or `/home` | Index, Privacy, Error | — |
| Admin | `/admin` | Index (login), Login, Logout | — |
| Plants | `/plants` | Index, Details, Create, Edit, Delete | searchTerm, webshopOnly, stage, healthStatus |
| Inventory | `/inventory` | Index, Details | searchTerm, webshopOnly |
| Care Profiles | `/care` | Index, Details, Create, Edit, Delete | searchTerm, requiredLight |
| Seed Batches | `/seeds` | Index, Details, Create, Edit, Delete | searchTerm, availableInWebshop |
| Taxonomy | `/taxonomy` | Index, Details, Create, Edit, Delete | searchTerm |

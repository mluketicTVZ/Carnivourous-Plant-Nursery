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

**Notes**: `Details()` returns `NotFound()` if no plant with the given id exists. Create/Edit require `Admin` or `Manager`; Delete requires `Admin`. Delete sets `TempData["DeleteError"]` and redirects to Details if a lineage entry references this plant. `Suggestions()` returns up to 8 matching results for the AJAX autocomplete control.

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

**Notes**: `Details()` returns `NotFound()` if id is not found. Create/Edit require `Admin` or `Manager`; Delete requires `Admin`. Delete sets `TempData["DeleteError"]` and redirects to Details if a taxonomy references this care profile.

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

**Notes**: `Details()` returns `NotFound()` if id is not found. Create/Edit require `Admin` or `Manager`; Delete requires `Admin`. Delete sets `TempData["DeleteError"]` and redirects to Details if a lineage entry references this seed batch. `Suggestions()` returns up to 8 matching results for the AJAX autocomplete control.

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

**Notes**: `Details()` returns `NotFound()` if id is not found. Create/Edit require `Admin` or `Manager`; Delete requires `Admin`. Delete sets `TempData["DeleteError"]` and redirects to Details if a plant or seed batch references this taxonomy. `Suggestions()` returns display-name matches for the search autocomplete; `IdSuggestions()` returns matches with the numeric ID as value, used by Plant/SeedBatch FK pickers.

---

## AccountController `[Route("account")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/account/login` | GET | AccountController | Login(string? returnUrl) | Views/Account/Login.cshtml |
| `/account/login` | POST | AccountController | Login(LoginViewModel model) | Redirects to return URL/Home on success, re-renders Login on failure |
| `/account/register` | GET | AccountController | Register() | Views/Account/Register.cshtml |
| `/account/register` | POST | AccountController | Register(RegisterViewModel model) | Creates a Customer account, signs in, redirects Home |
| `/account/logout` | POST | AccountController | Logout() | Signs out and redirects Home |

**Notes**: Authentication uses ASP.NET Core Identity with `AppUser`. Register requires email, display name, and password; phone number and default shipping city are optional. New registrations receive the `Customer` role.

---

## AdminController `[Route("admin")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/admin` | GET | AdminController | Index() | Redirects to `/account/login` |
| `/admin` | POST | AdminController | Login() | Redirects to `/account/login` |
| `/admin/logout` | POST | AdminController | Logout() | Signs out and redirects Home |

**Notes**: This controller is a compatibility bridge for old admin URLs. Inventory authorization no longer uses `HttpContext.Session["IsAdmin"]`; it uses Identity roles.

---

## API Controllers

API controllers return JSON DTOs and do not render Razor views. They use `[ApiController]` and are routed under `/api/...`.

### PlantApiController `[Route("api/plant")]`

| URL | HTTP | Action | Response |
|---|---|---|---|
| `/api/plant` | GET | GetAll(searchTerm, webshopOnly, stage, healthStatus) | `200 OK` with `PlantDto[]` |
| `/api/plant/{id}` | GET | GetById(int id) | `200 OK` with `PlantDto`, or `404 Not Found` |
| `/api/plant` | POST | Create(PlantWriteDto dto) | `201 Created` with `PlantDto`, or `400 Bad Request`; requires `Admin` or `Manager` |
| `/api/plant/{id}` | PUT | Update(int id, PlantWriteDto dto) | `200 OK` with `PlantDto`, `400 Bad Request`, or `404 Not Found`; requires `Admin` or `Manager` |
| `/api/plant/{id}` | DELETE | Delete(int id) | `204 No Content`, `404 Not Found`, or `409 Conflict` on business-rule delete failure; requires `Admin` |

### SeedBatchApiController `[Route("api/seeds")]`

| URL | HTTP | Action | Response |
|---|---|---|---|
| `/api/seeds` | GET | GetAll(searchTerm, availableInWebshop) | `200 OK` with `SeedBatchDto[]` |
| `/api/seeds/{id}` | GET | GetById(int id) | `200 OK` with `SeedBatchDto`, or `404 Not Found` |
| `/api/seeds` | POST | Create(SeedBatchWriteDto dto) | `201 Created` with `SeedBatchDto`, or `400 Bad Request`; requires `Admin` or `Manager` |
| `/api/seeds/{id}` | PUT | Update(int id, SeedBatchWriteDto dto) | `200 OK` with `SeedBatchDto`, `400 Bad Request`, or `404 Not Found`; requires `Admin` or `Manager` |
| `/api/seeds/{id}` | DELETE | Delete(int id) | `204 No Content`, `404 Not Found`, or `409 Conflict` on business-rule delete failure; requires `Admin` |

### TaxonomyApiController `[Route("api/taxonomy")]`

| URL | HTTP | Action | Response |
|---|---|---|---|
| `/api/taxonomy` | GET | GetAll(searchTerm) | `200 OK` with `TaxonomyDto[]` |
| `/api/taxonomy/{id}` | GET | GetById(int id) | `200 OK` with `TaxonomyDto`, or `404 Not Found` |
| `/api/taxonomy` | POST | Create(TaxonomyWriteDto dto) | `201 Created` with `TaxonomyDto`, or `400 Bad Request`; requires `Admin` or `Manager` |
| `/api/taxonomy/{id}` | PUT | Update(int id, TaxonomyWriteDto dto) | `200 OK` with `TaxonomyDto`, `400 Bad Request`, or `404 Not Found`; requires `Admin` or `Manager` |
| `/api/taxonomy/{id}` | DELETE | Delete(int id) | `204 No Content`, `404 Not Found`, or `409 Conflict` on business-rule delete failure; requires `Admin` |

### CareProfileApiController `[Route("api/care")]`

| URL | HTTP | Action | Response |
|---|---|---|---|
| `/api/care` | GET | GetAll(searchTerm, requiredLight) | `200 OK` with `CareProfileDto[]` |
| `/api/care/{id}` | GET | GetById(int id) | `200 OK` with `CareProfileDto`, or `404 Not Found` |
| `/api/care` | POST | Create(CareProfileWriteDto dto) | `201 Created` with `CareProfileDto`, or `400 Bad Request`; requires `Admin` or `Manager` |
| `/api/care/{id}` | PUT | Update(int id, CareProfileWriteDto dto) | `200 OK` with `CareProfileDto`, `400 Bad Request`, or `404 Not Found`; requires `Admin` or `Manager` |
| `/api/care/{id}` | DELETE | Delete(int id) | `204 No Content`, `404 Not Found`, or `409 Conflict` on business-rule delete failure; requires `Admin` |

### InventoryApiController `[Route("api/inventory")]`

| URL | HTTP | Action | Response |
|---|---|---|---|
| `/api/inventory` | GET | GetAll(searchTerm, webshopOnly) | `200 OK` with `InventoryItemSummaryDto[]` |
| `/api/inventory/{id}` | GET | GetById(int id) | `200 OK` with `InventoryItemSummaryDto`, or `404 Not Found` |

**Notes**: Inventory API is read-only because `InventoryItem` is abstract. Create/update/delete operations are exposed through concrete `/api/plant` and `/api/seeds` endpoints. API delete operations preserve existing soft-delete and business-rule checks. API read endpoints are public; API write endpoints use Identity role authorization.

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
| Account | `/account` | Login, Register, Logout | returnUrl |
| Admin compatibility | `/admin` | Redirect login, Logout | — |
| Plants | `/plants` | Index, Details, Create, Edit, Delete | searchTerm, webshopOnly, stage, healthStatus |
| Plant API | `/api/plant` | GET all, GET by ID, POST, PUT, DELETE | searchTerm, webshopOnly, stage, healthStatus |
| Inventory | `/inventory` | Index, Details | searchTerm, webshopOnly |
| Inventory API | `/api/inventory` | GET all, GET by ID | searchTerm, webshopOnly |
| Care Profiles | `/care` | Index, Details, Create, Edit, Delete | searchTerm, requiredLight |
| Care Profile API | `/api/care` | GET all, GET by ID, POST, PUT, DELETE | searchTerm, requiredLight |
| Seed Batches | `/seeds` | Index, Details, Create, Edit, Delete | searchTerm, availableInWebshop |
| Seed Batch API | `/api/seeds` | GET all, GET by ID, POST, PUT, DELETE | searchTerm, availableInWebshop |
| Taxonomy | `/taxonomy` | Index, Details, Create, Edit, Delete | searchTerm |
| Taxonomy API | `/api/taxonomy` | GET all, GET by ID, POST, PUT, DELETE | searchTerm |

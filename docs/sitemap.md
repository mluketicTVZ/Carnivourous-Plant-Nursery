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

**Notes**: `Details()` returns `NotFound()` if no plant with the given id exists.

---

## InventoryController `[Route("inventory")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/inventory` | GET | InventoryController | Index() | Views/Inventory/Index.cshtml |
| `/inventory?searchTerm=X` | GET | InventoryController | Index(string searchTerm) | Views/Inventory/Index.cshtml |
| `/inventory?webshopOnly=true` | GET | InventoryController | Index(bool webshopOnly) | Views/Inventory/Index.cshtml |
| `/inventory/{id}` | GET | InventoryController | Details(int id) | Views/Inventory/Details.cshtml |

**Notes**: Lists all inventory items (both Plants and SeedBatches). `Details()` returns `NotFound()` if id is not found.

---

## CareProfileController `[Route("care")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/care` | GET | CareProfileController | Index() | Views/CareProfile/Index.cshtml |
| `/care/{id}` | GET | CareProfileController | Details(int id) | Views/CareProfile/Details.cshtml |

**Notes**: `Details()` returns `NotFound()` if id is not found.

---

## SeedBatchController `[Route("seeds")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/seeds` | GET | SeedBatchController | Index() | Views/SeedBatch/Index.cshtml |
| `/seeds/{id}` | GET | SeedBatchController | Details(int id) | Views/SeedBatch/Details.cshtml |

**Notes**: `Details()` returns `NotFound()` if id is not found.

---

## TaxonomyController `[Route("taxonomy")]`

| URL | HTTP | Controller | Action | View |
|---|---|---|---|---|
| `/taxonomy` | GET | TaxonomyController | Index() | Views/Taxonomy/Index.cshtml |
| `/taxonomy?searchTerm=X` | GET | TaxonomyController | Index(string searchTerm) | Views/Taxonomy/Index.cshtml |
| `/taxonomy/{id}` | GET | TaxonomyController | Details(int id) | Views/Taxonomy/Details.cshtml |

**Notes**: `Details()` returns `NotFound()` if id is not found.

---

## Shared Partial Views

These are rendered inside other views and do not have their own URLs:

| Partial View | Used In | Purpose |
|---|---|---|
| Views/Shared/_Layout.cshtml | All views (via _ViewStart) | Master layout shell |
| Views/Shared/_BackButton.cshtml | Detail views | Back navigation button |
| Views/Shared/_ValidationScriptsPartial.cshtml | Form views | Client-side validation scripts |

---

## Summary Table

| Section | Base URL | Actions | Query Filters |
|---|---|---|---|
| Home | `/` or `/home` | Index, Privacy, Error | — |
| Plants | `/plants` | Index, Details | searchTerm, webshopOnly, stage, healthStatus |
| Inventory | `/inventory` | Index, Details | searchTerm, webshopOnly |
| Care Profiles | `/care` | Index, Details | — |
| Seed Batches | `/seeds` | Index, Details | — |
| Taxonomy | `/taxonomy` | Index, Details | searchTerm |

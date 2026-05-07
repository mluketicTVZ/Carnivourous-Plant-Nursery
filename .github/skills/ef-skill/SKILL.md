---
name: ef-skill
description: Use when working with Entity Framework Core in the Carnivorous Plant Nursery project — adding or modifying model classes with EF annotations, changing or adding repository queries, creating new EF repositories, generating or removing migrations, and inspecting or modifying AppDbContext. Keywords: EF, migration, DbContext, repository, LINQ, model, annotation, database, seeder.
---

# EF Core Skill — Carnivorous Plant Nursery

## Skill activation log

**Every time this skill is loaded**, append a single line to `agent_corner/skill_logs/ef-skill.txt` (relative to the solution root `c:\Brutal Stuff\Git\repo\mluketicTVZ\Carnivourous-Plant-Nursery\`) using the following PowerShell command before doing any other work:

```powershell
"$(Get-Date -Format 'yyyy-MM-ddTHH:mm:ss') - ef-skill loaded for: <brief task description>" | Add-Content -Path "agent_corner\skill_logs\ef-skill.txt"
```

Replace `<brief task description>` with a one-line summary of the EF task being performed.

---

Use this skill whenever you need to:
- Add or modify a model class
- Change a repository query
- Add a new EF repository
- Generate or remove a migration
- Inspect or modify `AppDbContext`

---

## Project identity

- **Solution root**: `c:\Brutal Stuff\Git\repo\mluketicTVZ\Carnivourous-Plant-Nursery\` — this is the absolute path on the developer's machine. All relative paths mentioned throughout this skill (e.g. `agent_corner\...`, `Carnivorous-Plant-Nursery\...`) are relative to this root. All CLI commands (`dotnet ef`, PowerShell) must be run from this directory.
- **Project folder**: `Carnivorous-Plant-Nursery\` (single-project, no separate DAL)
- **Root namespace**: `Carnivorous_Plant_Nursery` (underscores — project name has hyphens, namespace uses underscores)
- **Target framework**: .NET 8 / ASP.NET Core 8 MVC

---

## Database

- **Provider**: Microsoft SQL Server 2022 running in Docker
- **Docker command** (for reference only — do not run unless asked):
  ```
  docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Password123" -p 1433:1433 --name mssql-dev -d mcr.microsoft.com/mssql/server:2022-latest
  ```
- **Connection string** (`appsettings.json`):
  ```
  Server=localhost,1433;Database=CarnivorousPlantNursery;User Id=sa;Password=YourStrong!Password123;TrustServerCertificate=True;
  ```
- **Connection string key**: `DefaultConnection`

---

## DbContext

**File**: `Carnivorous-Plant-Nursery/Data/AppDbContext.cs`
**Class**: `AppDbContext : DbContext`

### DbSet properties (singular names — matches table names in DB)

```csharp
public DbSet<Plant> Plant { get; set; }
public DbSet<SeedBatch> SeedBatch { get; set; }
public DbSet<Taxonomy> Taxonomy { get; set; }
public DbSet<CareProfile> CareProfile { get; set; }
public DbSet<Lineage> Lineage { get; set; }
```

> There is **no** `DbSet<InventoryItem>` or `DbSet<Article>` — both are abstract base classes stored in the same `Article` table via TPH (Table-Per-Hierarchy). Use `DbSet<Plant>` and `DbSet<SeedBatch>` to query them.

### OnModelCreating — configured relationships and constraints

- `Lineage.Mother` → `InventoryItem` via `MotherId` — `OnDelete(Restrict)`
- `Lineage.Father` → `InventoryItem` via `FatherId` — `OnDelete(Restrict)`
- CHECK constraints (all via `.ToTable(t => t.HasCheckConstraint(...))`):
  - `CK_Article_Price_NonNegative`: `[Price] >= 0`
  - `CK_Plant_PotDiameterCm_NonNegative`, `CK_Plant_PotHeightCm_NonNegative`, `CK_Plant_EstimatedAgeAtAcquiryYears_NonNegative`
  - `CK_SeedBatch_SeedCount_NonNegative`, `CK_SeedBatch_EstimatedGerminationRate_Range`

---

## Model inheritance hierarchy (TPH — single `Article` table)

```
Article  (abstract)
└── InventoryItem  (abstract)
    ├── Plant
    └── SeedBatch
```

EF uses a `Discriminator` column (`nvarchar(13)`) to distinguish rows. The longest discriminator value is `"InventoryItem"` (13 chars) — but since `InventoryItem` is abstract it is never directly instantiated.

### Key models and their files

| Class | File | Notes |
|---|---|---|
| `Article` | `Models/Article.cs` | `[Key] Id`, `SKU`, `ListingTitle`, `Price` (decimal backing field ≥ 0), `IsAvailableInWebshop`, `Description` |
| `InventoryItem` | `Models/InventoryItem.cs` | Extends `Article`. Adds `DateAcquired`, `InternalNotes`, `LocationInNursery`, `TaxonomyId` + `virtual Taxonomy`, `LineageId` + `virtual Lineage` |
| `Plant` | `Models/Plant.cs` | Extends `InventoryItem`. Adds `CurrentStage` (enum), `PotDiameterCm`, `PotHeightCm` (decimal backing fields ≥ 0), `EstimatedAgeAtAcquiryYears`, `HealthStatus` (enum), `HealthDescription` |
| `SeedBatch` | `Models/SeedBatch.cs` | Extends `InventoryItem`. Adds `SeedCount`, `HarvestDate`, `RequiresStratification`, `EstimatedGerminationRate` (0–1), `ExpectedViabilityMonths` |
| `Taxonomy` | `Models/Taxonomy.cs` | `[Key] Id`, `Genus`, `Species`, `Cultivar`, `CommonName`, `CareProfileId` + `virtual CareProfile`, `virtual ICollection<InventoryItem> InventoryItems` |
| `CareProfile` | `Models/CareProfile.cs` | `[Key] Id`, care settings, `virtual ICollection<Taxonomy> Taxonomies` |
| `Lineage` | `Models/Lineage.cs` | `[Key] Id`, `MotherId`/`FatherId` (both nullable int FK → InventoryItem), `virtual Mother`/`virtual Father`, `Generation`, `IsClone`, `GeneticsDescription` |

---

## EF Data Annotations conventions

Always apply to model classes:
- `[Key]` on the `Id` property
- `[MaxLength(N)]` on all `string` properties (200 for names/titles, 1000 for descriptions, 50 for codes like SKU)
- `[Required]` only when the field must be non-null at the DB level
- `[ForeignKey("NavigationPropertyName")]` on every FK int property
- `[Column(TypeName = "decimal(18,2)")]` on every `decimal` property
- `virtual` on every navigation property and every `ICollection<T>`

---

## Repository pattern

- **Location**: `Carnivorous-Plant-Nursery/Repositories/`
- **Mock repos** (kept for reference, not used at runtime): `Repositories/MockRepositories/`
- **Active EF repos**: `PlantRepository`, `TaxonomyRepository`, `CareProfileRepository`, `SeedBatchRepository`, `InventoryRepository`
- All registered as **`AddScoped`** in `Program.cs` (must match `AppDbContext` lifetime)
- Injected via constructor: `public XxxRepository(AppDbContext db) { _db = db; }`

### LINQ query rules

1. Use `.Include(p => p.NavProp)` for every navigation property the caller needs — never lazy-load
2. Keep filtering (`.Where(...)`) **before** `.ToList()` so it runs in SQL
3. If a filter uses a computed C# property (e.g. `FullName`) or a method EF can't translate (e.g. `DateTime.AddMonths()`), call `.ToList()` first to bring data into memory, then filter in C#
4. Use `.FirstOrDefault(x => x.Id == id)` for single-item lookups — returns `null` if not found
5. `InventoryRepository.GetAll()` queries `Plant` and `SeedBatch` DbSets separately and combines with `.Concat()`, since there is no `DbSet<InventoryItem>`

---

## Migration CLI commands

Run from the **solution root** (`Carnivourous-Plant-Nursery\`):

```powershell
# Add a migration
dotnet ef migrations add <MigrationName> --project Carnivorous-Plant-Nursery --context AppDbContext

# Apply to database
dotnet ef database update --project Carnivorous-Plant-Nursery --context AppDbContext

# Remove last migration (only if not applied to DB yet)
dotnet ef migrations remove --project Carnivorous-Plant-Nursery --context AppDbContext

# Generate SQL script (for manual review)
dotnet ef migrations script --project Carnivorous-Plant-Nursery --context AppDbContext
```

**Migrations folder**: `Carnivorous-Plant-Nursery/Migrations/`
**Current migration**: `20260507101015_Initial`

> After every model change: (1) add a migration, (2) review the generated `.cs` file, (3) run `database update`.

---

## Startup seeder

**File**: `Carnivorous-Plant-Nursery/Data/DbSeeder.cs`
**Class**: `static DbSeeder`, method `Seed(AppDbContext db)`

- Guard: `if (db.CareProfile.Any()) return;` — runs only on empty database
- Insert order: CareProfiles → Taxonomies → Plants + SeedBatches (LineageId = null) → `SaveChanges()` → Lineages → `SaveChanges()` → back-fill LineageId on specific plants → `SaveChanges()`
- Uses `db.ChangeTracker.AutoDetectChangesEnabled = false` for performance; reset to `true` in `finally`
- Called in `Program.cs` inside a manual DI scope after `builder.Build()`

---

## Program.cs DI registration (relevant section)

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PlantRepository>();
builder.Services.AddScoped<TaxonomyRepository>();
builder.Services.AddScoped<CareProfileRepository>();
builder.Services.AddScoped<SeedBatchRepository>();
builder.Services.AddScoped<InventoryRepository>();

// after var app = builder.Build():
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}
```
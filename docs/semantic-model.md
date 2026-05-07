# Semantic DB Model — Carnivorous Plant Nursery

## Overview

The application manages a carnivorous plant nursery. The domain consists of plants and seed batches (inventory items), their taxonomic classification, care profiles, and genetic lineage.

---

## Classes / Tables

### Article *(abstract base — not a direct table)*
Base class for all inventory items.

| Property | Type | Notes |
|---|---|---|
| Id | int | [Key] Primary key |
| SKU | string? | Stock-keeping unit, max 50 chars |
| ListingTitle | string? | Webshop display name, max 200 chars |
| Price | decimal? | decimal(18,2) |
| IsAvailableInWebshop | bool | Whether listed in the webshop |
| Description | string? | Max 1000 chars |

---

### InventoryItem *(abstract, inherits Article — maps to table via TPH/TPT)*
Represents a physical item in the nursery.

| Property | Type | Notes |
|---|---|---|
| TaxonomyId | int? | [ForeignKey("Taxonomy")] |
| Taxonomy | Taxonomy? | Navigation — many InventoryItems → one Taxonomy |
| LineageId | int? | [ForeignKey("Lineage")] |
| Lineage | Lineage? | Navigation — many InventoryItems → one Lineage |
| DateAcquired | DateTime? | When the item was acquired |
| InternalNotes | string? | Max 500 chars |
| LocationInNursery | string? | Physical shelf/zone, max 200 chars |

---

### Plant *(inherits InventoryItem)*
Represents a living plant specimen.

| Property | Type | Notes |
|---|---|---|
| CurrentStage | PlantStage? | Enum: Seedling, Juvenile, Mature, Flowering, Dormant |
| PotDiameterCm | decimal? | decimal(18,2) |
| PotHeightCm | decimal? | decimal(18,2) |
| LastRepottingDate | DateTime? | — |
| LastDormancyDateStart | DateTime? | — |
| LastDormancyDateEnd | DateTime? | — |
| EstimatedAgeAtAcquiryYears | int? | — |
| HealthStatus | HealthState? | Enum: Excellent, Good, Fair, Poor, Quarantined, Dead |
| HealthDescription | string? | Max 1000 chars |

---

### SeedBatch *(inherits InventoryItem)*
Represents a batch of collected or purchased seeds.

| Property | Type | Notes |
|---|---|---|
| SeedCount | int? | Number of seeds in batch |
| HarvestDate | DateTime? | — |
| ExpectedViabilityMonths | int? | — |
| RequiresStratification | bool? | Whether cold stratification is needed |
| EstimatedGerminationRate | decimal? | 0.0 – 1.0, decimal(18,2) |

---

### Taxonomy
Represents the scientific/common classification of a plant species or cultivar.

| Property | Type | Notes |
|---|---|---|
| Id | int | [Key] Primary key |
| Genus | string? | Max 200 chars |
| Species | string? | Max 200 chars |
| Cultivar | string? | Max 200 chars |
| CommonName | string? | Max 200 chars |
| CareProfileId | int? | [ForeignKey("CareProfile")] |
| CareProfile | CareProfile? | Navigation — many Taxonomies → one CareProfile |
| FullName | string | Computed: Genus + Species + Cultivar (read-only) |
| InventoryItems | ICollection\<InventoryItem\> | Navigation — one Taxonomy → many InventoryItems |

---

### CareProfile
Describes the cultivation requirements for a taxonomy.

| Property | Type | Notes |
|---|---|---|
| Id | int | [Key] Primary key |
| CareProfileName | string | [Required] Max 200 chars |
| RequiredLight | LightLevel? | Enum: FullSun, PartialShade, BrightIndirect, LowLight |
| MinTemperature | TemperatureTolerance? | Enum: Freezing, Cold, Cool, Warm, Hot, Scorching |
| MaxTemperature | TemperatureTolerance? | Same enum |
| TemperatureDescription | string? | Max 500 chars |
| RequiresWinterDormancy | bool? | — |
| SoilMix | string? | Recommended soil mix, max 200 chars |
| RequiredHumidity | HumidityLevel? | Enum: Low, Moderate, High, ExtremelyHigh |
| CareDescription | string? | Free-text care notes, max 1000 chars |
| Taxonomies | ICollection\<Taxonomy\> | Navigation — one CareProfile → many Taxonomies |

---

### Lineage
Tracks genetic parentage between inventory items (cross-pollination / clones).

| Property | Type | Notes |
|---|---|---|
| Id | int | [Key] Primary key |
| MotherId | int? | [ForeignKey("Mother")] — references InventoryItem |
| Mother | InventoryItem? | Navigation — maternal parent |
| FatherId | int? | [ForeignKey("Father")] — references InventoryItem |
| Father | InventoryItem? | Navigation — paternal parent |
| Generation | string? | E.g. "F1", "F2", max 100 chars |
| IsClone | bool? | Whether this is a vegetative clone |
| GeneticsDescription | string? | Max 1000 chars |

---

## Enums

| Enum | Values |
|---|---|
| PlantStage | Seedling, Juvenile, Mature, Flowering, Dormant |
| LightLevel | FullSun, PartialShade, BrightIndirect, LowLight |
| HumidityLevel | Low, Moderate, High, ExtremelyHigh |
| HealthState | Excellent, Good, Fair, Poor, Quarantined, Dead |
| TemperatureTolerance | Freezing, Cold, Cool, Warm, Hot, Scorching |

---

## Relationships

```
CareProfile ──── 1:N ──── Taxonomy ──── 1:N ──── InventoryItem (Plant / SeedBatch)
                                                        │
                                                        │  (MotherId / FatherId)
                                                        └──────── N:1 ──── Lineage ──── N:1 ──── InventoryItem
```

| Relationship | Type | Description |
|---|---|---|
| CareProfile → Taxonomy | One-to-Many | One care profile applies to many taxonomies |
| Taxonomy → InventoryItem | One-to-Many | One taxonomy classifies many inventory items |
| InventoryItem → Lineage | Many-to-One | Many items may share the same lineage record |
| Lineage → InventoryItem (Mother) | Many-to-One | A lineage has one optional maternal parent |
| Lineage → InventoryItem (Father) | Many-to-One | A lineage has one optional paternal parent |
| InventoryItem (Plant) | TPH/TPT subtype | Plant-specific data |
| InventoryItem (SeedBatch) | TPH/TPT subtype | SeedBatch-specific data |

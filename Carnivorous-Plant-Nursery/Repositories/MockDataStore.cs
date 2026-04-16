using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Repositories
{
    /// <summary>
    /// Central static data store that seeds all mock data once.
    /// All mock repositories share the same object instances so that
    /// navigation properties (e.g. Plant.Taxonomy) are always populated.
    /// When the mock layer is replaced with a real database this class
    /// can simply be deleted.
    /// </summary>
    internal static class MockDataStore
    {
        // ── Care Profiles ────────────────────────────────────────────────────
        public static readonly List<CareProfile> CareProfiles = new()
        {
            new CareProfile
            {
                Id = 1,
                CareProfileName = "Dionaea Care",
                RequiredLight = LightLevel.FullSun,
                MinTemperature = TemperatureTolerance.Cold,
                MaxTemperature = TemperatureTolerance.Hot,
                TemperatureDescription = "Needs a cold winter dormancy period (0–10 °C).",
                RequiresWinterDormancy = true,
                SoilMix = "1:1 peat and perlite, no nutrients",
                RequiredHumidity = HumidityLevel.High,
                CareDescription = "Keep in full sun, use only distilled or rain water, tray method recommended."
            },
            new CareProfile
            {
                Id = 2,
                CareProfileName = "Sarracenia Care",
                RequiredLight = LightLevel.FullSun,
                MinTemperature = TemperatureTolerance.Freezing,
                MaxTemperature = TemperatureTolerance.Hot,
                TemperatureDescription = "Hardy; tolerates light frost during dormancy.",
                RequiresWinterDormancy = true,
                SoilMix = "1:1 peat and perlite",
                RequiredHumidity = HumidityLevel.High,
                CareDescription = "Bog plant; keep in standing water during growing season."
            },
            new CareProfile
            {
                Id = 3,
                CareProfileName = "Drosera Care",
                RequiredLight = LightLevel.BrightIndirect,
                MinTemperature = TemperatureTolerance.Cool,
                MaxTemperature = TemperatureTolerance.Warm,
                TemperatureDescription = "Prefers mild temperatures; avoid frost.",
                RequiresWinterDormancy = false,
                SoilMix = "2:1 peat and perlite",
                RequiredHumidity = HumidityLevel.High,
                CareDescription = "Keep moist at all times; bright indirect light is ideal."
            }
        };

        // ── Taxonomies ───────────────────────────────────────────────────────
        public static readonly List<Taxonomy> Taxonomies;

        // ── Lineages ─────────────────────────────────────────────────────────
        public static readonly List<Lineage> Lineages;

        // ── Plants ───────────────────────────────────────────────────────────
        public static readonly List<Plant> Plants;

        // ── Seed Batches ─────────────────────────────────────────────────────
        public static readonly List<SeedBatch> SeedBatches;

        // ── All Inventory Items (combined) ───────────────────────────────────
        public static readonly List<InventoryItem> AllInventoryItems;

        // ── Static constructor – runs once ───────────────────────────────────
        static MockDataStore()
        {
            // Resolve care profiles by id for convenience
            var care1 = CareProfiles.First(c => c.Id == 1);
            var care2 = CareProfiles.First(c => c.Id == 2);
            var care3 = CareProfiles.First(c => c.Id == 3);

            // Taxonomies
            var dionaea = new Taxonomy
            {
                Id = 1, Genus = "Dionaea", Species = "muscipula",
                Cultivar = "B52", CommonName = "Venerina muholovka",
                CareProfileId = 1, CareProfile = care1
            };
            var sarracenia = new Taxonomy
            {
                Id = 2, Genus = "Sarracenia", Species = "purpurea",
                Cultivar = "", CommonName = "Ljubičasta saracenija",
                CareProfileId = 2, CareProfile = care2
            };
            var drosera = new Taxonomy
            {
                Id = 3, Genus = "Drosera", Species = "capensis",
                Cultivar = "Alba", CommonName = "Kapska rosika",
                CareProfileId = 3, CareProfile = care3
            };
            var nepenthes = new Taxonomy
            {
                Id = 4, Genus = "Nepenthes", Species = "alata",
                Cultivar = "", CommonName = "Tropska vrčarica",
                CareProfileId = 3, CareProfile = care3
            };
            var pinguicula = new Taxonomy
            {
                Id = 5, Genus = "Pinguicula", Species = "moranensis",
                Cultivar = "", CommonName = "Maslaček hvatač",
                CareProfileId = 3, CareProfile = care3
            };

            Taxonomies = new List<Taxonomy> { dionaea, sarracenia, drosera, nepenthes, pinguicula };

            // Lineages
            var lineage1 = new Lineage
            {
                Id = 101, MotherId = 1, FatherId = 2,
                Generation = "F1", IsClone = false,
                GeneticsDescription = "Dionaea B52 × Sarracenia purpurea (experimental cross)"
            };
            var lineage2 = new Lineage
            {
                Id = 102, MotherId = 3, FatherId = null,
                Generation = "TC", IsClone = true,
                GeneticsDescription = "Tissue-culture clone of Drosera capensis 'Alba'"
            };

            Lineages = new List<Lineage> { lineage1, lineage2 };

            // Plants
            var plant1 = new Plant
            {
                Id = 1, TaxonomyId = 1, Taxonomy = dionaea,
                ListingTitle = "Velika Venerina muholovka (B52)", SKU = "PL-DIO-001",
                Price = 15.99m, IsAvailableInWebshop = true,
                Description = "Odrasla biljka klona B52 s iznimno velikim zamkama.",
                DateAcquired = new DateTime(2023, 4, 15),
                InternalNotes = "Oprez pri pakiranju – zamke su osjetljive.",
                LocationInNursery = "Staklenik sjever A",
                CurrentStage = PlantStage.Mature, HealthStatus = HealthState.Excellent,
                PotDiameterCm = 9.0m, PotHeightCm = 9.0m,
                EstimatedAgeAtAcquiryYears = 1,
                LineageId = 101, Lineage = lineage1
            };
            var plant2 = new Plant
            {
                Id = 2, TaxonomyId = 2, Taxonomy = sarracenia,
                ListingTitle = "Saracenija purpurea – Odrasla", SKU = "PL-SAR-001",
                Price = 22.50m, IsAvailableInWebshop = true,
                Description = "Jako obojena sorta purpuree s dubokim ljubičastim žilama.",
                DateAcquired = new DateTime(2022, 5, 10),
                LocationInNursery = "Staklenik jug B",
                CurrentStage = PlantStage.Flowering, HealthStatus = HealthState.Good,
                PotDiameterCm = 12.0m, PotHeightCm = 12.0m,
                EstimatedAgeAtAcquiryYears = 2
            };
            var plant3 = new Plant
            {
                Id = 3, TaxonomyId = 3, Taxonomy = drosera,
                ListingTitle = "Drosera capensis 'Alba' – Sadnica", SKU = "PL-DRO-002",
                Price = 5.00m, IsAvailableInWebshop = false,
                Description = "Male sadnice bijele rosike, još u fazi rasta.",
                DateAcquired = DateTime.Now.AddMonths(-2),
                LocationInNursery = "Polica pod lampama",
                CurrentStage = PlantStage.Seedling, HealthStatus = HealthState.Excellent,
                PotDiameterCm = 5.0m, PotHeightCm = 5.0m,
                EstimatedAgeAtAcquiryYears = 0,
                LineageId = 102, Lineage = lineage2
            };
            var plant4 = new Plant
            {
                Id = 4, TaxonomyId = 4, Taxonomy = nepenthes,
                ListingTitle = "Nepenthes alata – Mlada biljka", SKU = "PL-NEP-001",
                Price = 18.00m, IsAvailableInWebshop = true,
                Description = "Tropska vrčarica s već formiranim vrčevima.",
                DateAcquired = new DateTime(2024, 2, 20),
                LocationInNursery = "Terrarium odjel",
                CurrentStage = PlantStage.Juvenile, HealthStatus = HealthState.Good,
                PotDiameterCm = 10.0m, PotHeightCm = 12.0m,
                EstimatedAgeAtAcquiryYears = 1
            };
            var plant5 = new Plant
            {
                Id = 5, TaxonomyId = 5, Taxonomy = pinguicula,
                ListingTitle = "Pinguicula moranensis – Cvjetajuća", SKU = "PL-PIN-001",
                Price = 9.50m, IsAvailableInWebshop = true,
                Description = "Maslaček hvatač u punom cvatu, ružičasti cvjetovi.",
                DateAcquired = new DateTime(2023, 9, 1),
                LocationInNursery = "Polica pod lampama",
                CurrentStage = PlantStage.Flowering, HealthStatus = HealthState.Excellent,
                PotDiameterCm = 7.0m, PotHeightCm = 6.0m,
                EstimatedAgeAtAcquiryYears = 2
            };

            Plants = new List<Plant> { plant1, plant2, plant3, plant4, plant5 };

            // Seed Batches
            var seeds1 = new SeedBatch
            {
                Id = 6, TaxonomyId = 2, Taxonomy = sarracenia,
                ListingTitle = "Sjemenke S. purpurea (20 kom)", SKU = "SD-SAR-001",
                Price = 3.99m, IsAvailableInWebshop = true,
                Description = "Svježe sjemenke prikupljene prošle jeseni.",
                DateAcquired = new DateTime(2023, 10, 20),
                LocationInNursery = "Hladnjak C",
                SeedCount = 20, HarvestDate = new DateTime(2023, 10, 15),
                RequiresStratification = true, EstimatedGerminationRate = 0.85m,
                ExpectedViabilityMonths = 18
            };
            var seeds2 = new SeedBatch
            {
                Id = 7, TaxonomyId = 3, Taxonomy = drosera,
                ListingTitle = "Sjemenke D. capensis (100+)", SKU = "SD-DRO-001",
                Price = 2.50m, IsAvailableInWebshop = true,
                Description = "Mikroskopski sitno sjeme rosike, jako klijavo.",
                DateAcquired = new DateTime(2024, 1, 5),
                LocationInNursery = "Soba za klijanje",
                SeedCount = 150, HarvestDate = new DateTime(2023, 12, 10),
                RequiresStratification = false, EstimatedGerminationRate = 0.95m,
                ExpectedViabilityMonths = 12
            };
            var seeds3 = new SeedBatch
            {
                Id = 8, TaxonomyId = 1, Taxonomy = dionaea,
                ListingTitle = "Sjemenke D. muscipula 'B52' (10 kom)", SKU = "SD-DIO-001",
                Price = 6.99m, IsAvailableInWebshop = false,
                Description = "Rijetke sjemenke klona B52, ograničena količina.",
                DateAcquired = new DateTime(2024, 3, 1),
                LocationInNursery = "Hladnjak C",
                SeedCount = 10, HarvestDate = new DateTime(2024, 2, 20),
                RequiresStratification = false, EstimatedGerminationRate = 0.70m,
                ExpectedViabilityMonths = 6
            };

            SeedBatches = new List<SeedBatch> { seeds1, seeds2, seeds3 };

            // Wire up navigation collections on Taxonomy
            foreach (var plant in Plants)
                plant.Taxonomy?.InventoryItems.Add(plant);
            foreach (var seed in SeedBatches)
                seed.Taxonomy?.InventoryItems.Add(seed);

            AllInventoryItems = Plants.Cast<InventoryItem>()
                .Concat(SeedBatches.Cast<InventoryItem>())
                .ToList();
        }
    }
}

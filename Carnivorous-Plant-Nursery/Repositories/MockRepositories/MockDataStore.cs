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
            var heliamphora = new Taxonomy
            {
                Id = 6, Genus = "Heliamphora", Species = "nutans",
                Cultivar = "", CommonName = "Nutansov vrčar",
                CareProfileId = 3, CareProfile = care3
            };
            var darlingtonia = new Taxonomy
            {
                Id = 7, Genus = "Darlingtonia", Species = "californica",
                Cultivar = "", CommonName = "Kobrin ljiljan",
                CareProfileId = 2, CareProfile = care2
            };
            var droseraBinata = new Taxonomy
            {
                Id = 8, Genus = "Drosera", Species = "binata",
                Cultivar = "Multifida Extrema", CommonName = "Vilasta rosika",
                CareProfileId = 3, CareProfile = care3
            };
            var cephalotus = new Taxonomy
            {
                Id = 9, Genus = "Cephalotus", Species = "follicularis",
                Cultivar = "", CommonName = "Australska vrčarica",
                CareProfileId = 3, CareProfile = care3
            };

            Taxonomies = new List<Taxonomy> { dionaea, sarracenia, drosera, nepenthes, pinguicula, heliamphora, darlingtonia, droseraBinata, cephalotus };

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
            // IDs 6–8 are used by existing SeedBatches; new Plants continue from 9.
            var plant6 = new Plant
            {
                Id = 9, TaxonomyId = 6, Taxonomy = heliamphora,
                ListingTitle = "Heliamphora nutans – Odrasla", SKU = "PL-HEL-001",
                Price = 34.00m, IsAvailableInWebshop = true,
                Description = "Visočinska vrčarica s karakterističnim nektarijskim spoon-om na vrhu vrča.",
                DateAcquired = new DateTime(2024, 6, 10),
                LocationInNursery = "Terrarium odjel",
                CurrentStage = PlantStage.Mature, HealthStatus = HealthState.Excellent,
                PotDiameterCm = 11.0m, PotHeightCm = 13.0m,
                EstimatedAgeAtAcquiryYears = 3
            };
            var plant7 = new Plant
            {
                Id = 10, TaxonomyId = 7, Taxonomy = darlingtonia,
                ListingTitle = "Darlingtonia californica – Mlada biljka", SKU = "PL-DAR-001",
                Price = 19.99m, IsAvailableInWebshop = false,
                Description = "Kobrin ljiljan u karanteni nakon uvoza; nije dostupan za prodaju.",
                DateAcquired = new DateTime(2025, 3, 5),
                InternalNotes = "Karantena do lipnja 2025. – preventivni tretman štetočina.",
                LocationInNursery = "Karantenska komora K1",
                CurrentStage = PlantStage.Juvenile, HealthStatus = HealthState.Quarantined,
                PotDiameterCm = 9.0m, PotHeightCm = 10.0m,
                EstimatedAgeAtAcquiryYears = 1
            };
            var plant8 = new Plant
            {
                Id = 11, TaxonomyId = 8, Taxonomy = droseraBinata,
                ListingTitle = "Drosera binata 'Multifida Extrema' – Zrela", SKU = "PL-DRO-003",
                Price = 8.50m, IsAvailableInWebshop = true,
                Description = "Razgranata vilasta rosika s brojnim hvatačkim nitima; brzo raste.",
                DateAcquired = new DateTime(2024, 4, 18),
                LocationInNursery = "Polica pod lampama",
                CurrentStage = PlantStage.Mature, HealthStatus = HealthState.Good,
                PotDiameterCm = 10.0m, PotHeightCm = 10.0m,
                EstimatedAgeAtAcquiryYears = 2
            };
            var plant9 = new Plant
            {
                Id = 12, TaxonomyId = 9, Taxonomy = cephalotus,
                ListingTitle = "Cephalotus follicularis – Sadnica", SKU = "PL-CEP-001",
                Price = 45.00m, IsAvailableInWebshop = false,
                Description = "Iznimno rijetka australska vrčarica; sadnica još nije dovoljno razvijena za prodaju.",
                DateAcquired = DateTime.Now.AddMonths(-3),
                InternalNotes = "Strogo interno – čekati minimalno dvije godine rasta.",
                LocationInNursery = "Terrarium odjel",
                CurrentStage = PlantStage.Seedling, HealthStatus = HealthState.Good,
                PotDiameterCm = 6.0m, PotHeightCm = 6.0m,
                EstimatedAgeAtAcquiryYears = 0
            };

            Plants = new List<Plant> { plant1, plant2, plant3, plant4, plant5, plant6, plant7, plant8, plant9 };

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
            var seeds4 = new SeedBatch
            {
                Id = 13, TaxonomyId = 6, Taxonomy = heliamphora,
                ListingTitle = "Sjemenke H. nutans (5 kom)", SKU = "SD-HEL-001",
                Price = 12.00m, IsAvailableInWebshop = true,
                Description = "Rijetke sjemenke Heliamphorae; siju se svježe, ne čuvaju dobro.",
                DateAcquired = new DateTime(2024, 7, 1),
                LocationInNursery = "Hladnjak C",
                SeedCount = 5, HarvestDate = new DateTime(2024, 6, 25),
                RequiresStratification = false, EstimatedGerminationRate = 0.50m,
                ExpectedViabilityMonths = 3
            };
            var seeds5 = new SeedBatch
            {
                Id = 14, TaxonomyId = 7, Taxonomy = darlingtonia,
                ListingTitle = "Sjemenke D. californica (15 kom)", SKU = "SD-DAR-001",
                Price = 5.50m, IsAvailableInWebshop = false,
                Description = "Sjemenke kobrinog ljiljana; zahtijevaju hladnu stratifikaciju – batch još nije spreman za isporuku.",
                DateAcquired = new DateTime(2024, 11, 10),
                InternalNotes = "Stratifikacija traje do veljače 2025.",
                LocationInNursery = "Hladnjak C",
                SeedCount = 15, HarvestDate = new DateTime(2024, 10, 30),
                RequiresStratification = true, EstimatedGerminationRate = 0.60m,
                ExpectedViabilityMonths = 24
            };
            var seeds6 = new SeedBatch
            {
                Id = 15, TaxonomyId = 8, Taxonomy = droseraBinata,
                ListingTitle = "Sjemenke D. binata 'M.E.' (200+)", SKU = "SD-DRO-002",
                Price = 3.00m, IsAvailableInWebshop = true,
                Description = "Sitno, lako klijavo sjeme vilaste rosike; idealno za početnike.",
                DateAcquired = new DateTime(2025, 1, 20),
                LocationInNursery = "Soba za klijanje",
                SeedCount = 250, HarvestDate = new DateTime(2025, 1, 10),
                RequiresStratification = false, EstimatedGerminationRate = 0.90m,
                ExpectedViabilityMonths = 12
            };
            var seeds7 = new SeedBatch
            {
                Id = 16, TaxonomyId = 9, Taxonomy = cephalotus,
                ListingTitle = "Sjemenke C. follicularis (8 kom)", SKU = "SD-CEP-001",
                Price = 18.00m, IsAvailableInWebshop = false,
                Description = "Iznimno rijetke sjemenke australske vrčarice; namijenjene isključivo internom razmnožavanju.",
                DateAcquired = new DateTime(2025, 2, 14),
                InternalNotes = "Nije za prodaju – interni razmnožavački program.",
                LocationInNursery = "Hladnjak C",
                SeedCount = 8, HarvestDate = new DateTime(2025, 2, 1),
                RequiresStratification = false, EstimatedGerminationRate = 0.35m,
                ExpectedViabilityMonths = 4
            };

            SeedBatches = new List<SeedBatch> { seeds1, seeds2, seeds3, seeds4, seeds5, seeds6, seeds7 };

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

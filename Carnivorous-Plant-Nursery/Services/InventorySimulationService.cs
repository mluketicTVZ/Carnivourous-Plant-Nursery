using Carnivorous_Plant_Nursery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carnivorous_Plant_Nursery.Services
{
    public class InventorySimulationService
    {
        public async Task<List<Taxonomy>> FetchInventoryAsync()
        {
            await Task.Delay(500); // reduced delay for bette UX
            
            var dionaeaTaxonomy = new Taxonomy
            {
                Id = 1, Genus = "Dionaea", Species = "muscipula",
                Cultivar = "B52", CommonName = "Venerina muholovka", CareProfileId = 1
            };
            
            var sarraceniaTaxonomy = new Taxonomy
            {
                Id = 2, Genus = "Sarracenia", Species = "purpurea",
                Cultivar = "", CommonName = "Ljubičasta saracenija", CareProfileId = 2 // Fixed non-ascii characters for proper display
            };

            var droseraTaxonomy = new Taxonomy
            {
                Id = 3, Genus = "Drosera", Species = "capensis",
                Cultivar = "Alba", CommonName = "Kapska rosika", CareProfileId = 3
            };

            var lineage1 = new Lineage { Id = 101, MotherId = 1, FatherId = 2, Generation = "F1", IsClone = false, GeneticsDescription = "Dionaea B52 x Typical" };
            
            var plant1 = new Plant
            {
                Id = 1, TaxonomyId = 1, Taxonomy = dionaeaTaxonomy, 
                ListingTitle = "Velika Venerina muholovka (B52)", SKU = "PL-DIO-001",
                Price = 15.99m, IsAvailableInWebshop = true, Description = "Odrasla biljka klona B52.",
                DateAcquired = new DateTime(2023, 4, 15), InternalNotes = "Oprez pri pakiranju", LocationInNursery = "Staklenik sjev A",
                CurrentStage = PlantStage.Mature, HealthStatus = HealthState.Excellent,
                PotDiameterCm = 9.0m, PotHeightCm = 9.0m, EstimatedAgeAtAcquiryYears = 1,
                LineageId = 101, Lineage = lineage1
            };

            var plant2 = new Plant
            {
                Id = 2, TaxonomyId = 2, Taxonomy = sarraceniaTaxonomy,
                ListingTitle = "Saracenija purpurea - Odrasla", SKU = "PL-SAR-001",
                Price = 22.50m, IsAvailableInWebshop = true, Description = "Jako obojena sorta purpuree.",
                DateAcquired = new DateTime(2022, 5, 10), LocationInNursery = "Staklenik jug B",
                CurrentStage = PlantStage.Flowering, HealthStatus = HealthState.Good,
                PotDiameterCm = 12.0m, PotHeightCm = 12.0m, EstimatedAgeAtAcquiryYears = 2
            };

            var plant3 = new Plant
            {
                Id = 3, TaxonomyId = 3, Taxonomy = droseraTaxonomy,
                ListingTitle = "Drosera capensis 'Alba' Seedling", SKU = "PL-DRO-002",
                Price = 5.00m, IsAvailableInWebshop = false, Description = "Male sadnice drosere, još rastu.",
                DateAcquired = DateTime.Now, LocationInNursery = "Polica pod lampama",
                CurrentStage = PlantStage.Seedling, HealthStatus = HealthState.Excellent,
                PotDiameterCm = 5.0m, PotHeightCm = 5.0m, EstimatedAgeAtAcquiryYears = 0
            };

            var seeds1 = new SeedBatch
            {
                Id = 4, TaxonomyId = 2, Taxonomy = sarraceniaTaxonomy,
                ListingTitle = "Sjemenke S. purpurea (20 kom)", SKU = "SD-SAR-001",
                Price = 3.99m, IsAvailableInWebshop = true, Description = "Svježe sjemenke prikupljene prošle jeseni.",
                DateAcquired = new DateTime(2023, 10, 20), LocationInNursery = "Hladnjak C",
                SeedCount = 20, HarvestDate = new DateTime(2023, 10, 15),
                RequiresStratification = true, EstimatedGerminationRate = 0.85m
            };

            var seeds2 = new SeedBatch
            {
                Id = 5, TaxonomyId = 3, Taxonomy = droseraTaxonomy,
                ListingTitle = "Sjemenke D. capensis (100+)", SKU = "SD-DRO-001",
                Price = 2.50m, IsAvailableInWebshop = true, Description = "Mikroskopski sitno sjeme rosike, jako klijavo.",
                DateAcquired = new DateTime(2024, 1, 5), LocationInNursery = "Soba za klijanje",
                SeedCount = 150, HarvestDate = new DateTime(2023, 12, 10),
                RequiresStratification = false, EstimatedGerminationRate = 0.95m
            };

            dionaeaTaxonomy.InventoryItems = new List<InventoryItem>();
            sarraceniaTaxonomy.InventoryItems = new List<InventoryItem>();
            droseraTaxonomy.InventoryItems = new List<InventoryItem>();

            dionaeaTaxonomy.InventoryItems.Add(plant1);
            sarraceniaTaxonomy.InventoryItems.Add(plant2);
            sarraceniaTaxonomy.InventoryItems.Add(seeds1);
            droseraTaxonomy.InventoryItems.Add(plant3);
            droseraTaxonomy.InventoryItems.Add(seeds2);

            return new List<Taxonomy> { dionaeaTaxonomy, sarraceniaTaxonomy, droseraTaxonomy };
        }

        public List<InventoryItem> GetWebshopItems(List<Taxonomy> taxonomies)
        {
            return taxonomies
                .SelectMany(t => t.InventoryItems)
                .Where(item => item.IsAvailableInWebshop)
                .OrderBy(item => item.Price)
                .ToList();
        }

        public List<SeedBatch> GetSeedsRequiringStratification(List<Taxonomy> taxonomies)
        {
            return taxonomies
                .SelectMany(t => t.InventoryItems)
                .OfType<SeedBatch>()
                .Where(s => s.RequiresStratification == true)
                .OrderByDescending(s => s.SeedCount)
                .ToList();
        }

        public List<InventoryItem> GetItemsWithKnownLineage(List<Taxonomy> taxonomies)
        {
            return taxonomies
                .SelectMany(t => t.InventoryItems)
                .Where(i => i.Lineage != null && (i.Lineage.MotherId != null || i.Lineage.FatherId != null))
                .ToList();
        }
    }
}
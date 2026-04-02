using System.Collections.Generic;

namespace Carnivorous_Plant_Nursery.Models
{
    public class HomeIndexViewModel
    {
        public List<Taxonomy> Taxonomies { get; set; } = new List<Taxonomy>();
        public List<InventoryItem> WebshopItems { get; set; } = new List<InventoryItem>();
        public List<SeedBatch> SeedsForStratification { get; set; } = new List<SeedBatch>();
        public List<InventoryItem> LineagePlants { get; set; } = new List<InventoryItem>();
    }
}
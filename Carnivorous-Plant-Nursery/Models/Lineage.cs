using System;

namespace Carnivorous_Plant_Nursery.Models
{
    public class Lineage
    {
        public int Id { get; set; }
        public int? MotherId { get; set; }
        public InventoryItem? Mother { get; set; }
        public int? FatherId { get; set; }
        public InventoryItem? Father { get; set; }
        public string? Generation { get; set; }
        public bool? IsClone { get; set; }
        public string? GeneticsDescription { get; set; }
    }
}

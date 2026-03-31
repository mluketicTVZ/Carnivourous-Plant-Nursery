using System;

namespace Carnivorous_Plant_Nursery.Models
{
    public abstract class InventoryItem : Article
    {
        public int TaxonomyId { get; set; }
        public Taxonomy Taxonomy { get; set; }
        public int? LineageId { get; set; }
        public Lineage? Lineage { get; set; }
        public DateTime DateAcquired { get; set; }
        public string InternalNotes { get; set; }
        public string LocationInNursery { get; set; }
    }
}
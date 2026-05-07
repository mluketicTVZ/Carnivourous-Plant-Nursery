using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models
{
    public abstract class InventoryItem : Article
    {
        [ForeignKey("Taxonomy")]
        public int? TaxonomyId { get; set; }
        public virtual Taxonomy? Taxonomy { get; set; }

        [ForeignKey("Lineage")]
        public int? LineageId { get; set; }
        public virtual Lineage? Lineage { get; set; }

        public DateTime? DateAcquired { get; set; }

        [MaxLength(500)]
        public string? InternalNotes { get; set; }

        [MaxLength(200)]
        public string? LocationInNursery { get; set; }

        protected InventoryItem() {}
    }
}

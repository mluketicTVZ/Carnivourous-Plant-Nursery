using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models.Api
{
    public class InventoryItemSummaryDto
    {
        public int Id { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string? ListingTitle { get; set; }
        public decimal? Price { get; set; }
        public bool IsAvailableInWebshop { get; set; }
        public int? TaxonomyId { get; set; }
        public TaxonomySummaryDto? Taxonomy { get; set; }
    }

    public abstract class InventoryItemWriteDto
    {
        [MaxLength(50)]
        public string? SKU { get; set; }

        [MaxLength(200)]
        public string? ListingTitle { get; set; }

        [Range(typeof(decimal), "0", "999999.99")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public bool IsAvailableInWebshop { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int? TaxonomyId { get; set; }
        public int? LineageId { get; set; }
        public DateTime? DateAcquired { get; set; }

        [MaxLength(500)]
        public string? InternalNotes { get; set; }

        [MaxLength(200)]
        public string? LocationInNursery { get; set; }
    }
}

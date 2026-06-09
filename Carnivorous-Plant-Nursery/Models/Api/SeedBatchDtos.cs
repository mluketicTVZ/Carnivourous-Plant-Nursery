using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models.Api
{
    public class SeedBatchDto : InventoryItemSummaryDto
    {
        public string? Description { get; set; }
        public int? LineageId { get; set; }
        public DateTime? DateAcquired { get; set; }
        public string? InternalNotes { get; set; }
        public string? LocationInNursery { get; set; }
        public int? SeedCount { get; set; }
        public DateTime? HarvestDate { get; set; }
        public int? ExpectedViabilityMonths { get; set; }
        public bool? RequiresStratification { get; set; }
        public decimal? EstimatedGerminationRate { get; set; }
    }

    public class SeedBatchWriteDto : InventoryItemWriteDto
    {
        [Range(0, int.MaxValue)]
        public int? SeedCount { get; set; }

        public DateTime? HarvestDate { get; set; }
        public int? ExpectedViabilityMonths { get; set; }
        public bool? RequiresStratification { get; set; }

        [Range(typeof(decimal), "0", "1")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedGerminationRate { get; set; }
    }
}

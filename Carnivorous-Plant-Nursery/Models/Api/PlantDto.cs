using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models.Api
{
    public class PlantDto : InventoryItemSummaryDto
    {
        public string? Description { get; set; }
        public int? LineageId { get; set; }
        public DateTime? DateAcquired { get; set; }
        public string? InternalNotes { get; set; }
        public string? LocationInNursery { get; set; }
        public PlantStage? CurrentStage { get; set; }
        public decimal? PotDiameterCm { get; set; }
        public decimal? PotHeightCm { get; set; }
        public DateTime? LastRepottingDate { get; set; }
        public DateTime? LastDormancyDateStart { get; set; }
        public DateTime? LastDormancyDateEnd { get; set; }
        public int? EstimatedAgeAtAcquiryYears { get; set; }
        public HealthState? HealthStatus { get; set; }
        public string? HealthDescription { get; set; }
        public int? SourceSeedBatchId { get; set; }
    }

    public class PlantWriteDto : InventoryItemWriteDto
    {
        public PlantStage? CurrentStage { get; set; }

        [Range(typeof(decimal), "0", "999.99")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PotDiameterCm { get; set; }

        [Range(typeof(decimal), "0", "999.99")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PotHeightCm { get; set; }

        public DateTime? LastRepottingDate { get; set; }
        public DateTime? LastDormancyDateStart { get; set; }
        public DateTime? LastDormancyDateEnd { get; set; }

        [Range(0, 1000)]
        public int? EstimatedAgeAtAcquiryYears { get; set; }

        public HealthState? HealthStatus { get; set; }

        [MaxLength(1000)]
        public string? HealthDescription { get; set; }

        public int? SourceSeedBatchId { get; set; }
    }
}

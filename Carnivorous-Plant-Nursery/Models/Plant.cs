using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models
{
    public class Plant : InventoryItem
    {
        public PlantStage? CurrentStage { get; set; }

        private decimal? _potDiameterCm;
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PotDiameterCm
        {
            get => _potDiameterCm;
            set
            {
                if (value.HasValue && value.Value < 0m)
                    throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage.NegativePotDiameter);
                _potDiameterCm = value;
            }
        }

        private decimal? _potHeightCm;
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PotHeightCm
        {
            get => _potHeightCm;
            set
            {
                if (value.HasValue && value.Value < 0m)
                    throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage.NegativePotHeight);
                _potHeightCm = value;
            }
        }

        public DateTime? LastRepottingDate { get; set; }
        public DateTime? LastDormancyDateStart { get; set; }
        public DateTime? LastDormancyDateEnd { get; set; }
        private int? _estimatedAgeAtAcquiryYears;
        public int? EstimatedAgeAtAcquiryYears
        {
            get => _estimatedAgeAtAcquiryYears;
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage.NegativeEstimatedAge);
                _estimatedAgeAtAcquiryYears = value;
            }
        }

        public HealthState? HealthStatus { get; set; }

        [MaxLength(1000)]
        public string? HealthDescription { get; set; }
    }
}

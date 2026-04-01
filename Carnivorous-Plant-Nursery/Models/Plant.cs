using System;

namespace Carnivorous_Plant_Nursery.Models
{
    public class Plant : InventoryItem
    {
        public PlantStage? CurrentStage { get; set; }
        public decimal? PotDiameterCm { get; set; }
        public decimal? PotHeightCm { get; set; }
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
        public string? HealthDescription { get; set; }
    }
}

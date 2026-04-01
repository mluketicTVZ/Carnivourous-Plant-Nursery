using System;

namespace Carnivorous_Plant_Nursery.Models
{
    public class SeedBatch : InventoryItem
    {
        private int? _seedCount;
        public int? SeedCount
        {
            get => _seedCount;
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage.NegativeSeedCount);
                _seedCount = value;
            }
        }

        public DateTime? HarvestDate { get; set; }
        public int? ExpectedViabilityMonths { get; set; }
        public bool? RequiresStratification { get; set; }

        private decimal? _estimatedGerminationRate;
        public decimal? EstimatedGerminationRate
        {
            get => _estimatedGerminationRate;
            set
            {
                if (value.HasValue && (value.Value < 0m || value.Value > 1m))
                    throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage.InvalidGerminationRate);
                _estimatedGerminationRate = value;
            }
        }
    }
}

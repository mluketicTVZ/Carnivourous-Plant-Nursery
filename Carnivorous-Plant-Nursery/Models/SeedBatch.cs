using System;

namespace Carnivorous_Plant_Nursery.Models
{
    public class SeedBatch : InventoryItem
    {
        private int _seedCount;
        public int SeedCount
        {
            get => _seedCount;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Seed count cannot be negative.");
                _seedCount = value;
            }
        }

        public DateTime HarvestDate { get; set; }
        public int? ExpectedViabilityMonths { get; set; }
        public bool RequiresStratification { get; set; }

        private decimal _estimatedGerminationRate;
        public decimal EstimatedGerminationRate
        {
            get => _estimatedGerminationRate;
            set
            {
                if (value < 0m || value > 1m)
                    throw new ArgumentOutOfRangeException(nameof(value), "Germination rate must be between 0.0 and 1.0.");
                _estimatedGerminationRate = value;
            }
        }
    }
}
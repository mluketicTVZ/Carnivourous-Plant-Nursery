using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Repositories
{
    /// <summary>
    /// Mock repository for <see cref="SeedBatch"/> data.
    /// Returns pre-seeded in-memory data from <see cref="MockDataStore"/>.
    /// Replace this class with a real EF Core / database implementation
    /// without changing any controller code.
    /// </summary>
    public class SeedBatchMockRepository
    {
        private readonly List<SeedBatch> _data = MockDataStore.SeedBatches;

        /// <summary>Returns all seed batches.</summary>
        public List<SeedBatch> GetAll() => _data;

        /// <summary>Returns a single seed batch by its primary key, or null if not found.</summary>
        public SeedBatch? GetById(int id) =>
            _data.FirstOrDefault(s => s.Id == id);

        /// <summary>Returns all seed batches currently listed in the webshop.</summary>
        public List<SeedBatch> GetAvailableInWebshop() =>
            _data
                .Where(s => s.IsAvailableInWebshop)
                .OrderBy(s => s.Price)
                .ToList();

        /// <summary>Returns all seed batches belonging to a specific taxonomy.</summary>
        public List<SeedBatch> GetByTaxonomy(int taxonomyId) =>
            _data
                .Where(s => s.TaxonomyId == taxonomyId)
                .ToList();

        /// <summary>
        /// Returns seed batches that require cold stratification before sowing.
        /// Ordered by seed count descending so the largest batches appear first.
        /// </summary>
        public List<SeedBatch> GetRequiringStratification() =>
            _data
                .Where(s => s.RequiresStratification == true)
                .OrderByDescending(s => s.SeedCount)
                .ToList();

        /// <summary>
        /// Returns seed batches whose estimated germination rate is at or above
        /// the given threshold (0.0 – 1.0).
        /// </summary>
        public List<SeedBatch> GetByMinGerminationRate(decimal minRate) =>
            _data
                .Where(s => s.EstimatedGerminationRate.HasValue &&
                            s.EstimatedGerminationRate.Value >= minRate)
                .OrderByDescending(s => s.EstimatedGerminationRate)
                .ToList();

        /// <summary>
        /// Returns seed batches that are still within their expected viability window
        /// (harvest date + viability months is in the future).
        /// </summary>
        public List<SeedBatch> GetViable() =>
            _data
                .Where(s =>
                    s.HarvestDate.HasValue &&
                    s.ExpectedViabilityMonths.HasValue &&
                    s.HarvestDate.Value.AddMonths(s.ExpectedViabilityMonths.Value) >= DateTime.Today)
                .ToList();

        /// <summary>
        /// Returns seed batches ordered by harvest date, newest first.
        /// Useful for a "freshest seeds" section.
        /// </summary>
        public List<SeedBatch> GetRecentlyHarvested(int count = 5) =>
            _data
                .Where(s => s.HarvestDate.HasValue)
                .OrderByDescending(s => s.HarvestDate)
                .Take(count)
                .ToList();
    }
}

using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Repositories
{
    /// <summary>
    /// Mock repository for <see cref="Plant"/> data.
    /// Returns pre-seeded in-memory data from <see cref="MockDataStore"/>.
    /// Replace this class with a real EF Core / database implementation
    /// without changing any controller code.
    /// </summary>
    public class PlantMockRepository
    {
        private readonly List<Plant> _data = MockDataStore.Plants;

        /// <summary>Returns all plants.</summary>
        public List<Plant> GetAll() => _data;

        /// <summary>Returns a single plant by its primary key, or null if not found.</summary>
        public Plant? GetById(int id) =>
            _data.FirstOrDefault(p => p.Id == id);

        /// <summary>Returns all plants that are currently listed in the webshop.</summary>
        public List<Plant> GetAvailableInWebshop() =>
            _data
                .Where(p => p.IsAvailableInWebshop)
                .OrderBy(p => p.Price)
                .ToList();

        /// <summary>Returns all plants belonging to a specific taxonomy.</summary>
        public List<Plant> GetByTaxonomy(int taxonomyId) =>
            _data
                .Where(p => p.TaxonomyId == taxonomyId)
                .ToList();

        /// <summary>Returns all plants that have a known lineage record.</summary>
        public List<Plant> GetWithKnownLineage() =>
            _data
                .Where(p => p.Lineage != null &&
                            (p.Lineage.MotherId != null || p.Lineage.FatherId != null))
                .ToList();

        /// <summary>Returns all plants filtered by their current growth stage.</summary>
        public List<Plant> GetByStage(PlantStage stage) =>
            _data
                .Where(p => p.CurrentStage == stage)
                .ToList();

        /// <summary>Returns all plants filtered by their health status.</summary>
        public List<Plant> GetByHealthStatus(HealthState status) =>
            _data
                .Where(p => p.HealthStatus == status)
                .ToList();

        /// <summary>
        /// Returns plants whose listing title or description contains the
        /// given search term (case-insensitive).
        /// </summary>
        public List<Plant> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            var term = searchTerm.Trim().ToLowerInvariant();
            return _data
                .Where(p =>
                    (p.ListingTitle?.ToLowerInvariant().Contains(term) ?? false) ||
                    (p.Description?.ToLowerInvariant().Contains(term) ?? false) ||
                    (p.Taxonomy?.FullName.ToLowerInvariant().Contains(term) ?? false))
                .ToList();
        }

        /// <summary>
        /// Returns plants ordered by acquisition date, newest first.
        /// Useful for a "recently added" section on the home page.
        /// </summary>
        public List<Plant> GetRecentlyAcquired(int count = 5) =>
            _data
                .Where(p => p.DateAcquired.HasValue)
                .OrderByDescending(p => p.DateAcquired)
                .Take(count)
                .ToList();
    }
}

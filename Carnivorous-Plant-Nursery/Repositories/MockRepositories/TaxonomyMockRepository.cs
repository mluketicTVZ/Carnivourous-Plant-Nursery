using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Repositories
{
    /// <summary>
    /// Mock repository for <see cref="Taxonomy"/> data.
    /// Returns pre-seeded in-memory data from <see cref="MockDataStore"/>.
    /// Replace this class with a real EF Core / database implementation
    /// without changing any controller code.
    /// </summary>
    public class TaxonomyMockRepository
    {
        private readonly List<Taxonomy> _data = MockDataStore.Taxonomies;

        /// <summary>Returns all taxonomy entries.</summary>
        public List<Taxonomy> GetAll() => _data;

        /// <summary>Returns a single taxonomy by its primary key, or null if not found.</summary>
        public Taxonomy? GetById(int id) =>
            _data.FirstOrDefault(t => t.Id == id);

        /// <summary>
        /// Searches taxonomies whose genus, species, cultivar or common name
        /// contains the given search term (case-insensitive).
        /// </summary>
        public List<Taxonomy> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            var term = searchTerm.Trim().ToLowerInvariant();
            return _data
                .Where(t =>
                    (t.Genus?.ToLowerInvariant().Contains(term) ?? false) ||
                    (t.Species?.ToLowerInvariant().Contains(term) ?? false) ||
                    (t.Cultivar?.ToLowerInvariant().Contains(term) ?? false) ||
                    (t.CommonName?.ToLowerInvariant().Contains(term) ?? false))
                .ToList();
        }

        /// <summary>
        /// Returns all taxonomies that have at least one inventory item
        /// currently available in the webshop.
        /// </summary>
        public List<Taxonomy> GetWithWebshopItems() =>
            _data
                .Where(t => t.InventoryItems.Any(i => i.IsAvailableInWebshop))
                .ToList();

        /// <summary>
        /// Returns all taxonomies that require winter dormancy
        /// (based on their linked care profile).
        /// </summary>
        public List<Taxonomy> GetRequiringDormancy() =>
            _data
                .Where(t => t.CareProfile?.RequiresWinterDormancy == true)
                .ToList();
    }
}

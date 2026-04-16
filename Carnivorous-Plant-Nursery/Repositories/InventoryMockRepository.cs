using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Repositories
{
    /// <summary>
    /// Mock repository that works across the full <see cref="InventoryItem"/> hierarchy
    /// (both <see cref="Plant"/> and <see cref="SeedBatch"/>).
    /// Mirrors the LINQ queries that were originally defined in Program.cs.
    /// Replace this class with a real EF Core / database implementation
    /// without changing any controller code.
    /// </summary>
    public class InventoryMockRepository
    {
        private readonly List<InventoryItem> _data = MockDataStore.AllInventoryItems;

        /// <summary>Returns every inventory item regardless of type.</summary>
        public List<InventoryItem> GetAll() => _data;

        /// <summary>Returns a single inventory item by its primary key, or null if not found.</summary>
        public InventoryItem? GetById(int id) =>
            _data.FirstOrDefault(i => i.Id == id);

        // ── Webshop ──────────────────────────────────────────────────────────

        /// <summary>
        /// Returns all items currently available in the webshop,
        /// ordered by price ascending.
        /// Mirrors the original <c>GetWebshopItems</c> LINQ query from Program.cs.
        /// </summary>
        public List<InventoryItem> GetWebshopItems() =>
            _data
                .Where(i => i.IsAvailableInWebshop)
                .OrderBy(i => i.Price)
                .ToList();

        // ── Lineage ──────────────────────────────────────────────────────────

        /// <summary>
        /// Returns all items that have a known lineage (at least one parent recorded).
        /// Mirrors the original <c>GetItemsWithKnownLineage</c> LINQ query from Program.cs.
        /// </summary>
        public List<InventoryItem> GetItemsWithKnownLineage() =>
            _data
                .Where(i => i.Lineage != null &&
                            (i.Lineage.MotherId != null || i.Lineage.FatherId != null))
                .ToList();

        // ── Type filtering ───────────────────────────────────────────────────

        /// <summary>Returns only the <see cref="Plant"/> items from the inventory.</summary>
        public List<Plant> GetAllPlants() =>
            _data.OfType<Plant>().ToList();

        /// <summary>Returns only the <see cref="SeedBatch"/> items from the inventory.</summary>
        public List<SeedBatch> GetAllSeedBatches() =>
            _data.OfType<SeedBatch>().ToList();

        // ── Taxonomy grouping ────────────────────────────────────────────────

        /// <summary>
        /// Returns all inventory items for a specific taxonomy,
        /// ordered by type (plants first) then by listing title.
        /// </summary>
        public List<InventoryItem> GetByTaxonomy(int taxonomyId) =>
            _data
                .Where(i => i.TaxonomyId == taxonomyId)
                .OrderBy(i => i is Plant ? 0 : 1)
                .ThenBy(i => i.ListingTitle)
                .ToList();

        // ── Stratification ───────────────────────────────────────────────────

        /// <summary>
        /// Returns seed batches that require cold stratification.
        /// Mirrors the original <c>GetSeedsRequiringStratification</c> LINQ query from Program.cs.
        /// </summary>
        public List<SeedBatch> GetSeedsRequiringStratification() =>
            _data
                .OfType<SeedBatch>()
                .Where(s => s.RequiresStratification == true)
                .OrderByDescending(s => s.SeedCount)
                .ToList();

        // ── Statistics / summary ─────────────────────────────────────────────

        /// <summary>
        /// Returns a summary count of plants vs seed batches in the inventory.
        /// Useful for a dashboard / home page widget.
        /// </summary>
        public (int PlantCount, int SeedBatchCount) GetInventorySummary() =>
            (_data.OfType<Plant>().Count(), _data.OfType<SeedBatch>().Count());

        /// <summary>
        /// Returns the total estimated value of all webshop-listed items
        /// (sum of prices).
        /// </summary>
        public decimal GetWebshopTotalValue() =>
            _data
                .Where(i => i.IsAvailableInWebshop && i.Price.HasValue)
                .Sum(i => i.Price!.Value);

        // ── Search ───────────────────────────────────────────────────────────

        /// <summary>
        /// Full-text search across listing title, description and taxonomy name
        /// for all inventory item types (case-insensitive).
        /// </summary>
        public List<InventoryItem> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            var term = searchTerm.Trim().ToLowerInvariant();
            return _data
                .Where(i =>
                    (i.ListingTitle?.ToLowerInvariant().Contains(term) ?? false) ||
                    (i.Description?.ToLowerInvariant().Contains(term) ?? false) ||
                    (i.Taxonomy?.FullName.ToLowerInvariant().Contains(term) ?? false))
                .ToList();
        }
    }
}

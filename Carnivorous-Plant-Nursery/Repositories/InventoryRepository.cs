using Carnivorous_Plant_Nursery.Data;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Repositories
{
    public class InventoryRepository
    {
        private readonly AppDbContext _db;

        public InventoryRepository(AppDbContext db)
        {
            _db = db;
        }

        public List<InventoryItem> GetAll()
        {
            var plants = _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .Where(p => p.DeletedAt == null)
                .ToList()
                .Cast<InventoryItem>();

            var seeds = _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .Where(s => s.DeletedAt == null)
                .ToList()
                .Cast<InventoryItem>();

            return plants.Concat(seeds).ToList();
        }

        public async Task<List<InventoryItem>> GetAllAsync() =>
            await SearchAsync(null, null);

        public InventoryItem? GetById(int id)
        {
            InventoryItem? item = _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .FirstOrDefault(p => p.Id == id && p.DeletedAt == null);

            if (item != null) return item;

            return _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .FirstOrDefault(s => s.Id == id && s.DeletedAt == null);
        }

        public async Task<InventoryItem?> GetByIdAsync(int id)
        {
            InventoryItem? item = await _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (item != null) return item;

            return await _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);
        }

        public async Task<List<InventoryItem>> SearchAsync(string? searchTerm, bool? webshopOnly)
        {
            var plantQuery = _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .Where(p => p.DeletedAt == null);

            var seedQuery = _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .Where(s => s.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();

                plantQuery = plantQuery.Where(i =>
                    (i.ListingTitle != null && i.ListingTitle.Contains(term)) ||
                    (i.SKU != null && i.SKU.Contains(term)) ||
                    (i.Taxonomy != null && (
                        (i.Taxonomy.CommonName != null && i.Taxonomy.CommonName.Contains(term)) ||
                        (i.Taxonomy.Genus != null && i.Taxonomy.Genus.Contains(term)) ||
                        (i.Taxonomy.Species != null && i.Taxonomy.Species.Contains(term)) ||
                        (i.Taxonomy.Cultivar != null && i.Taxonomy.Cultivar.Contains(term))
                    )));

                seedQuery = seedQuery.Where(i =>
                    (i.ListingTitle != null && i.ListingTitle.Contains(term)) ||
                    (i.SKU != null && i.SKU.Contains(term)) ||
                    (i.Taxonomy != null && (
                        (i.Taxonomy.CommonName != null && i.Taxonomy.CommonName.Contains(term)) ||
                        (i.Taxonomy.Genus != null && i.Taxonomy.Genus.Contains(term)) ||
                        (i.Taxonomy.Species != null && i.Taxonomy.Species.Contains(term)) ||
                        (i.Taxonomy.Cultivar != null && i.Taxonomy.Cultivar.Contains(term))
                    )));
            }

            if (webshopOnly == true)
            {
                plantQuery = plantQuery.Where(i => i.IsAvailableInWebshop);
                seedQuery = seedQuery.Where(i => i.IsAvailableInWebshop);
            }

            var plants = await plantQuery.ToListAsync();
            var seeds = await seedQuery.ToListAsync();

            return plants.Cast<InventoryItem>().Concat(seeds).ToList();
        }

        public List<InventoryItem> GetWebshopItems()
        {
            var plants = _db.Plant
                .Include(p => p.Taxonomy)
                .Where(p => p.DeletedAt == null && p.IsAvailableInWebshop)
                .ToList()
                .Cast<InventoryItem>();

            var seeds = _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Where(s => s.DeletedAt == null && s.IsAvailableInWebshop)
                .ToList()
                .Cast<InventoryItem>();

            return plants.Concat(seeds).OrderBy(i => i.Price).ToList();
        }

        public List<InventoryItem> GetItemsWithKnownLineage()
        {
            var plants = _db.Plant
                .Include(p => p.Lineage)
                .Where(p => p.DeletedAt == null &&
                            p.Lineage != null &&
                            (p.Lineage.MotherId != null || p.Lineage.FatherId != null))
                .ToList()
                .Cast<InventoryItem>();

            var seeds = _db.SeedBatch
                .Include(s => s.Lineage)
                .Where(s => s.DeletedAt == null &&
                            s.Lineage != null &&
                            (s.Lineage.MotherId != null || s.Lineage.FatherId != null))
                .ToList()
                .Cast<InventoryItem>();

            return plants.Concat(seeds).ToList();
        }

        public List<Plant> GetAllPlants() =>
            _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .Where(p => p.DeletedAt == null)
                .ToList();

        public List<SeedBatch> GetAllSeedBatches() =>
            _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .Where(s => s.DeletedAt == null)
                .ToList();
    }
}

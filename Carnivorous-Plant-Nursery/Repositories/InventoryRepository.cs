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
                .ToList()
                .Cast<InventoryItem>();

            var seeds = _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .ToList()
                .Cast<InventoryItem>();

            return plants.Concat(seeds).ToList();
        }

        public InventoryItem? GetById(int id)
        {
            InventoryItem? item = _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .FirstOrDefault(p => p.Id == id);

            if (item != null) return item;

            return _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .FirstOrDefault(s => s.Id == id);
        }

        public List<InventoryItem> GetWebshopItems()
        {
            var plants = _db.Plant
                .Include(p => p.Taxonomy)
                .Where(p => p.IsAvailableInWebshop)
                .ToList()
                .Cast<InventoryItem>();

            var seeds = _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Where(s => s.IsAvailableInWebshop)
                .ToList()
                .Cast<InventoryItem>();

            return plants.Concat(seeds).OrderBy(i => i.Price).ToList();
        }

        public List<InventoryItem> GetItemsWithKnownLineage()
        {
            var plants = _db.Plant
                .Include(p => p.Lineage)
                .Where(p => p.Lineage != null &&
                            (p.Lineage.MotherId != null || p.Lineage.FatherId != null))
                .ToList()
                .Cast<InventoryItem>();

            var seeds = _db.SeedBatch
                .Include(s => s.Lineage)
                .Where(s => s.Lineage != null &&
                            (s.Lineage.MotherId != null || s.Lineage.FatherId != null))
                .ToList()
                .Cast<InventoryItem>();

            return plants.Concat(seeds).ToList();
        }

        public List<Plant> GetAllPlants() =>
            _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .ToList();

        public List<SeedBatch> GetAllSeedBatches() =>
            _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .ToList();
    }
}

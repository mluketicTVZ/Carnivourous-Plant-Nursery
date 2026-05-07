using Carnivorous_Plant_Nursery.Data;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Repositories
{
    public class TaxonomyRepository
    {
        private readonly AppDbContext _db;

        public TaxonomyRepository(AppDbContext db)
        {
            _db = db;
        }

        public List<Taxonomy> GetAll() =>
            _db.Taxonomy
                .Include(t => t.CareProfile)
                .Include(t => t.InventoryItems)
                .ToList();

        public Taxonomy? GetById(int id) =>
            _db.Taxonomy
                .Include(t => t.CareProfile)
                .Include(t => t.InventoryItems)
                .FirstOrDefault(t => t.Id == id);

        public List<Taxonomy> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            var term = searchTerm.Trim();
            return _db.Taxonomy
                .Include(t => t.CareProfile)
                .Include(t => t.InventoryItems)
                .Where(t =>
                    (t.Genus != null && t.Genus.Contains(term)) ||
                    (t.Species != null && t.Species.Contains(term)) ||
                    (t.Cultivar != null && t.Cultivar.Contains(term)) ||
                    (t.CommonName != null && t.CommonName.Contains(term)))
                .ToList();
        }

        public List<Taxonomy> GetWithWebshopItems() =>
            _db.Taxonomy
                .Include(t => t.InventoryItems)
                .Where(t => t.InventoryItems.Any(i => i.IsAvailableInWebshop))
                .ToList();

        public List<Taxonomy> GetRequiringDormancy() =>
            _db.Taxonomy
                .Include(t => t.CareProfile)
                .Where(t => t.CareProfile != null && t.CareProfile.RequiresWinterDormancy == true)
                .ToList();

        public void Add(Taxonomy taxonomy)
        {
            _db.Taxonomy.Add(taxonomy);
            _db.SaveChanges();
        }

        public void Update(Taxonomy taxonomy)
        {
            _db.Taxonomy.Update(taxonomy);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _db.Taxonomy.Find(id);
            if (entity != null)
            {
                bool hasPlants = _db.Plant.Any(p => p.TaxonomyId == id);
                bool hasSeedBatches = _db.SeedBatch.Any(s => s.TaxonomyId == id);
                if (hasPlants || hasSeedBatches)
                    throw new InvalidOperationException(
                        "This taxonomy cannot be deleted because plants or seed batches are assigned to it. Remove or reassign those records first.");

                _db.Taxonomy.Remove(entity);
                _db.SaveChanges();
            }
        }
    }
}

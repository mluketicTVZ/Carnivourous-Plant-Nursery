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
    }
}

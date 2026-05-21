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

        public async Task<List<Taxonomy>> GetAll() =>
            await _db.Taxonomy
                .Include(t => t.CareProfile)
                .Include(t => t.InventoryItems)
                .Where(t => t.DeletedAt == null)
                .ToListAsync();

        public async Task<Taxonomy?> GetById(int id) =>
            await _db.Taxonomy
                .Include(t => t.CareProfile)
                .Include(t => t.InventoryItems)
                .Where(t => t.DeletedAt == null)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<List<Taxonomy>> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAll();

            var term = searchTerm.Trim();
            return await _db.Taxonomy
                .Include(t => t.CareProfile)
                .Include(t => t.InventoryItems)
                .Where(t =>
                    t.DeletedAt == null &&
                    ((t.Genus != null && t.Genus.Contains(term)) ||
                    (t.Species != null && t.Species.Contains(term)) ||
                    (t.Cultivar != null && t.Cultivar.Contains(term)) ||
                    (t.CommonName != null && t.CommonName.Contains(term))))
                .ToListAsync();
        }

        public async Task<List<Taxonomy>> GetWithWebshopItems() =>
            await _db.Taxonomy
                .Include(t => t.InventoryItems)
                .Where(t => t.DeletedAt == null)
                .Where(t => t.InventoryItems.Any(i => i.IsAvailableInWebshop))
                .ToListAsync();

        public async Task<List<Taxonomy>> GetRequiringDormancy() =>
            await _db.Taxonomy
                .Include(t => t.CareProfile)
                .Where(t => t.DeletedAt == null)
                .Where(t => t.CareProfile != null && t.CareProfile.RequiresWinterDormancy == true)
                .ToListAsync();

        public async Task Add(Taxonomy taxonomy)
        {
            _db.Taxonomy.Add(taxonomy);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Taxonomy taxonomy)
        {
            _db.Taxonomy.Update(taxonomy);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.Taxonomy.FindAsync(id);
            if (entity != null)
            {
                bool hasPlants = await _db.Plant.AnyAsync(p => p.TaxonomyId == id);
                bool hasSeedBatches = await _db.SeedBatch.AnyAsync(s => s.TaxonomyId == id);
                if (hasPlants || hasSeedBatches)
                    throw new InvalidOperationException(ErrorMessage.DeleteErrorTaxonomyHasItems);

                entity.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
    }
}

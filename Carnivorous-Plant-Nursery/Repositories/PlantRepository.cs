using Carnivorous_Plant_Nursery.Data;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Repositories
{
    public class PlantRepository
    {
        private readonly AppDbContext _db;

        public PlantRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Plant>> GetAll() =>
            await _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .Where(p => p.DeletedAt == null)
                .ToListAsync();

        public async Task<Plant?> GetById(int id) =>
            await _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .Where(p => p.DeletedAt == null)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<List<Plant>> GetAvailableInWebshop() =>
            await _db.Plant
                .Include(p => p.Taxonomy)
                .Where(p => p.DeletedAt == null)
                .Where(p => p.IsAvailableInWebshop)
                .OrderBy(p => p.Price)
                .ToListAsync();

        public async Task<List<Plant>> GetByTaxonomy(int taxonomyId) =>
            await _db.Plant
                .Where(p => p.DeletedAt == null)
                .Where(p => p.TaxonomyId == taxonomyId)
                .ToListAsync();

        public async Task<List<Plant>> GetWithKnownLineage() =>
            await _db.Plant
                .Include(p => p.Lineage)
                .Where(p => p.DeletedAt == null)
                .Where(p => p.Lineage != null &&
                            (p.Lineage.MotherId != null || p.Lineage.FatherId != null))
                .ToListAsync();

        public async Task<List<Plant>> GetByStage(PlantStage stage) =>
            await _db.Plant
                .Where(p => p.DeletedAt == null)
                .Where(p => p.CurrentStage == stage)
                .ToListAsync();

        public async Task<List<Plant>> GetByHealthStatus(HealthState status) =>
            await _db.Plant
                .Where(p => p.DeletedAt == null)
                .Where(p => p.HealthStatus == status)
                .ToListAsync();

        public async Task<List<Plant>> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAll();

            var term = searchTerm.Trim();
            return await _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .Where(p =>
                    p.DeletedAt == null &&
                    ((p.ListingTitle != null && p.ListingTitle.Contains(term)) ||
                    (p.Description != null && p.Description.Contains(term)) ||
                    (p.Taxonomy != null && (
                        (p.Taxonomy.Genus != null && p.Taxonomy.Genus.Contains(term)) ||
                        (p.Taxonomy.Species != null && p.Taxonomy.Species.Contains(term)) ||
                        (p.Taxonomy.Cultivar != null && p.Taxonomy.Cultivar.Contains(term))
                    ))))
                .ToListAsync();
        }
        public async Task Add(Plant plant)
        {
            _db.Plant.Add(plant);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Plant plant)
        {
            _db.Plant.Update(plant);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.Plant.FindAsync(id);
            if (entity != null)
            {
                bool usedInLineage = await _db.Lineage.AnyAsync(l => l.MotherId == id || l.FatherId == id);
                if (usedInLineage)
                    throw new InvalidOperationException(ErrorMessage.DeleteErrorPlantInLineage);

                entity.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }    }
}

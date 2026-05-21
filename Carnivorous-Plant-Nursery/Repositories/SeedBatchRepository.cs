using Carnivorous_Plant_Nursery.Data;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Repositories
{
    public class SeedBatchRepository
    {
        private readonly AppDbContext _db;

        public SeedBatchRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<SeedBatch>> GetAll() =>
            await _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Where(s => s.DeletedAt == null)
                .ToListAsync();

        public async Task<SeedBatch?> GetById(int id) =>
            await _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .Where(s => s.DeletedAt == null)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<List<SeedBatch>> GetAvailableInWebshop() =>
            await _db.SeedBatch
                .Where(s => s.DeletedAt == null)
                .Where(s => s.IsAvailableInWebshop)
                .OrderBy(s => s.Price)
                .ToListAsync();

        public async Task<List<SeedBatch>> GetByTaxonomy(int taxonomyId) =>
            await _db.SeedBatch
                .Where(s => s.DeletedAt == null)
                .Where(s => s.TaxonomyId == taxonomyId)
                .ToListAsync();

        public async Task<List<SeedBatch>> GetRequiringStratification() =>
            await _db.SeedBatch
                .Where(s => s.DeletedAt == null)
                .Where(s => s.RequiresStratification == true)
                .OrderByDescending(s => s.SeedCount)
                .ToListAsync();

        public async Task<List<SeedBatch>> GetByMinGerminationRate(decimal minRate) =>
            await _db.SeedBatch
                .Where(s => s.DeletedAt == null)
                .Where(s => s.EstimatedGerminationRate.HasValue &&
                            s.EstimatedGerminationRate.Value >= minRate)
                .OrderByDescending(s => s.EstimatedGerminationRate)
                .ToListAsync();

        public async Task<List<SeedBatch>> GetViable() =>
            await _db.SeedBatch
                .Where(s => s.DeletedAt == null)
                .Where(s =>
                    s.HarvestDate.HasValue &&
                    s.ExpectedViabilityMonths.HasValue &&
                    s.HarvestDate.Value.AddMonths(s.ExpectedViabilityMonths.Value) >= DateTime.Today)
                .ToListAsync();
        public async Task Add(SeedBatch seedBatch)
        {
            _db.SeedBatch.Add(seedBatch);
            await _db.SaveChangesAsync();
        }

        public async Task Update(SeedBatch seedBatch)
        {
            _db.SeedBatch.Update(seedBatch);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.SeedBatch.FindAsync(id);
            if (entity != null)
            {
                bool usedInLineage = await _db.Lineage.AnyAsync(l => l.MotherId == id || l.FatherId == id);
                if (usedInLineage)
                    throw new InvalidOperationException(ErrorMessage.DeleteErrorSeedBatchInLineage);

                bool usedAsPlantSource = await _db.Plant.AnyAsync(p => p.SourceSeedBatchId == id && p.DeletedAt == null);
                if (usedAsPlantSource)
                    throw new InvalidOperationException(ErrorMessage.DeleteErrorSeedBatchHasPlants);

                entity.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }    }
}

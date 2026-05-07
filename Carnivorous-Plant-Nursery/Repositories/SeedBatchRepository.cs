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

        public List<SeedBatch> GetAll() =>
            _db.SeedBatch
                .Include(s => s.Taxonomy)
                .ToList();

        public SeedBatch? GetById(int id) =>
            _db.SeedBatch
                .Include(s => s.Taxonomy)
                .Include(s => s.Lineage)
                .FirstOrDefault(s => s.Id == id);

        public List<SeedBatch> GetAvailableInWebshop() =>
            _db.SeedBatch
                .Where(s => s.IsAvailableInWebshop)
                .OrderBy(s => s.Price)
                .ToList();

        public List<SeedBatch> GetByTaxonomy(int taxonomyId) =>
            _db.SeedBatch
                .Where(s => s.TaxonomyId == taxonomyId)
                .ToList();

        public List<SeedBatch> GetRequiringStratification() =>
            _db.SeedBatch
                .Where(s => s.RequiresStratification == true)
                .OrderByDescending(s => s.SeedCount)
                .ToList();

        public List<SeedBatch> GetByMinGerminationRate(decimal minRate) =>
            _db.SeedBatch
                .Where(s => s.EstimatedGerminationRate.HasValue &&
                            s.EstimatedGerminationRate.Value >= minRate)
                .OrderByDescending(s => s.EstimatedGerminationRate)
                .ToList();

        /// <summary>
        /// Returns seed batches still within their expected viability window.
        /// AddMonths cannot be translated to SQL, so data is pulled to memory first.
        /// </summary>
        public List<SeedBatch> GetViable() =>
            _db.SeedBatch
                .ToList()
                .Where(s =>
                    s.HarvestDate.HasValue &&
                    s.ExpectedViabilityMonths.HasValue &&
                    s.HarvestDate.Value.AddMonths(s.ExpectedViabilityMonths.Value) >= DateTime.Today)
                .ToList();
    }
}

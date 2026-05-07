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

        public List<Plant> GetAll() =>
            _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .ToList();

        public Plant? GetById(int id) =>
            _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .FirstOrDefault(p => p.Id == id);

        public List<Plant> GetAvailableInWebshop() =>
            _db.Plant
                .Include(p => p.Taxonomy)
                .Where(p => p.IsAvailableInWebshop)
                .OrderBy(p => p.Price)
                .ToList();

        public List<Plant> GetByTaxonomy(int taxonomyId) =>
            _db.Plant
                .Where(p => p.TaxonomyId == taxonomyId)
                .ToList();

        public List<Plant> GetWithKnownLineage() =>
            _db.Plant
                .Include(p => p.Lineage)
                .Where(p => p.Lineage != null &&
                            (p.Lineage.MotherId != null || p.Lineage.FatherId != null))
                .ToList();

        public List<Plant> GetByStage(PlantStage stage) =>
            _db.Plant
                .Where(p => p.CurrentStage == stage)
                .ToList();

        public List<Plant> GetByHealthStatus(HealthState status) =>
            _db.Plant
                .Where(p => p.HealthStatus == status)
                .ToList();

        public List<Plant> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            var term = searchTerm.Trim();
            return _db.Plant
                .Include(p => p.Taxonomy)
                .Include(p => p.Lineage)
                .Where(p =>
                    (p.ListingTitle != null && p.ListingTitle.Contains(term)) ||
                    (p.Description != null && p.Description.Contains(term)) ||
                    (p.Taxonomy != null && (
                        (p.Taxonomy.Genus != null && p.Taxonomy.Genus.Contains(term)) ||
                        (p.Taxonomy.Species != null && p.Taxonomy.Species.Contains(term)) ||
                        (p.Taxonomy.Cultivar != null && p.Taxonomy.Cultivar.Contains(term))
                    )))
                .ToList();
        }
    }
}

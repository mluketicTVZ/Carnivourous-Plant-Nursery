using Carnivorous_Plant_Nursery.Data;
using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Repositories
{
    public class CareProfileRepository
    {
        private readonly AppDbContext _db;

        public CareProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public List<CareProfile> GetAll() =>
            _db.CareProfile.ToList();

        public CareProfile? GetById(int id) =>
            _db.CareProfile.FirstOrDefault(c => c.Id == id);

        public void Add(CareProfile careProfile)
        {
            _db.CareProfile.Add(careProfile);
            _db.SaveChanges();
        }

        public void Update(CareProfile careProfile)
        {
            _db.CareProfile.Update(careProfile);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _db.CareProfile.Find(id);
            if (entity != null)
            {
                bool hasReferences = _db.Taxonomy.Any(t => t.CareProfileId == id);
                if (hasReferences)
                    throw new InvalidOperationException(
                        "This care profile cannot be deleted because one or more taxonomies are assigned to it. Remove or reassign those taxonomies first.");

                _db.CareProfile.Remove(entity);
                _db.SaveChanges();
            }
        }
    }
}

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
    }
}

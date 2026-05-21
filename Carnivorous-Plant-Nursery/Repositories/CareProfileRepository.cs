using Carnivorous_Plant_Nursery.Data;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Repositories
{
    public class CareProfileRepository
    {
        private readonly AppDbContext _db;

        public CareProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CareProfile>> GetAll() =>
            await _db.CareProfile
                .Where(c => c.DeletedAt == null)
                .ToListAsync();

        public async Task<CareProfile?> GetById(int id) =>
            await _db.CareProfile
                .Where(c => c.DeletedAt == null)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task Add(CareProfile careProfile)
        {
            _db.CareProfile.Add(careProfile);
            await _db.SaveChangesAsync();
        }

        public async Task Update(CareProfile careProfile)
        {
            _db.CareProfile.Update(careProfile);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.CareProfile.FindAsync(id);
            if (entity != null)
            {
                bool hasReferences = await _db.Taxonomy.AnyAsync(t => t.CareProfileId == id);
                if (hasReferences)
                    throw new InvalidOperationException(ErrorMessage.DeleteErrorCareProfileHasTaxonomies);

                entity.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
    }
}

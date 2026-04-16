using Carnivorous_Plant_Nursery.Models;

namespace Carnivorous_Plant_Nursery.Repositories
{
    /// <summary>
    /// Mock repository for <see cref="CareProfile"/> data.
    /// Returns pre-seeded in-memory data from <see cref="MockDataStore"/>.
    /// Replace this class with a real EF Core / database implementation
    /// without changing any controller code.
    /// </summary>
    public class CareProfileMockRepository
    {
        private readonly List<CareProfile> _data = MockDataStore.CareProfiles;

        /// <summary>Returns all care profiles.</summary>
        public List<CareProfile> GetAll() => _data;

        /// <summary>Returns a single care profile by its primary key, or null if not found.</summary>
        public CareProfile? GetById(int id) =>
            _data.FirstOrDefault(c => c.Id == id);
    }
}

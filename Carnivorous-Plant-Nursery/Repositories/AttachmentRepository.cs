using Carnivorous_Plant_Nursery.Data;
using Carnivorous_Plant_Nursery.Models;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Repositories
{
    public class AttachmentRepository
    {
        private readonly AppDbContext _db;

        public AttachmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Attachment>> GetActiveForPlant(int plantId) =>
            await _db.Attachment
                .Where(a => a.PlantId == plantId)
                .Where(a => a.DeletedAt == null)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<List<Attachment>> GetActiveForSeedBatch(int seedBatchId) =>
            await _db.Attachment
                .Where(a => a.SeedBatchId == seedBatchId)
                .Where(a => a.DeletedAt == null)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task Add(Attachment attachment)
        {
            _db.Attachment.Add(attachment);
            await _db.SaveChangesAsync();
        }

        public async Task AddRange(IEnumerable<Attachment> attachments)
        {
            _db.Attachment.AddRange(attachments);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> SoftDelete(int plantId, int attachmentId)
        {
            var attachment = await _db.Attachment
                .Where(a => a.PlantId == plantId)
                .Where(a => a.DeletedAt == null)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);

            if (attachment == null)
                return false;

            attachment.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SoftDeleteMany(int plantId, IEnumerable<int> attachmentIds)
        {
            var ids = attachmentIds.Distinct().ToList();
            if (ids.Count == 0)
                return;

            var attachments = await _db.Attachment
                .Where(a => a.PlantId == plantId)
                .Where(a => a.DeletedAt == null)
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();

            foreach (var attachment in attachments)
            {
                attachment.DeletedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public async Task SoftDeleteManyForSeedBatch(int seedBatchId, IEnumerable<int> attachmentIds)
        {
            var ids = attachmentIds.Distinct().ToList();
            if (ids.Count == 0)
                return;

            var attachments = await _db.Attachment
                .Where(a => a.SeedBatchId == seedBatchId)
                .Where(a => a.DeletedAt == null)
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();

            foreach (var attachment in attachments)
            {
                attachment.DeletedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }
    }
}

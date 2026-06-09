using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }

        public int? PlantId { get; set; }

        [ForeignKey("PlantId")]
        public virtual Plant? Plant { get; set; }

        public int? SeedBatchId { get; set; }

        [ForeignKey("SeedBatchId")]
        public virtual SeedBatch? SeedBatch { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string StoredFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }
    }
}

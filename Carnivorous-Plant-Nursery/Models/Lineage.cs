using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models
{
    public class Lineage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Mother")]
        public int? MotherId { get; set; }
        public virtual InventoryItem? Mother { get; set; }

        [ForeignKey("Father")]
        public int? FatherId { get; set; }
        public virtual InventoryItem? Father { get; set; }

        [MaxLength(100)]
        public string? Generation { get; set; }

        public bool? IsClone { get; set; }

        [MaxLength(1000)]
        public string? GeneticsDescription { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models
{
    public class Taxonomy
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Genus { get; set; }

        [MaxLength(200)]
        public string? Species { get; set; }

        [MaxLength(200)]
        public string? Cultivar { get; set; }

        [MaxLength(200)]
        public string? CommonName { get; set; }

        [ForeignKey("CareProfile")]
        public int? CareProfileId { get; set; }
        public virtual CareProfile? CareProfile { get; set; }

        public string FullName 
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(Genus)) parts.Add(Genus);
                if (!string.IsNullOrEmpty(Species)) parts.Add(Species);
                if (!string.IsNullOrEmpty(Cultivar)) parts.Add($"'{Cultivar}'");
                
                return parts.Count > 0 ? string.Join(" ", parts) : DisplayConstant.UnknownFullName;
            }
        }

        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carnivorous_Plant_Nursery.Models
{
    public class CareProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string CareProfileName { get; set; } = string.Empty;

        public LightLevel? RequiredLight { get; set; }
        public TemperatureTolerance? MinTemperature { get; set; }
        public TemperatureTolerance? MaxTemperature { get; set; }

        [MaxLength(500)]
        public string? TemperatureDescription { get; set; }

        public bool? RequiresWinterDormancy { get; set; }

        [MaxLength(200)]
        public string? SoilMix { get; set; }

        public HumidityLevel? RequiredHumidity { get; set; }

        [MaxLength(1000)]
        public string? CareDescription { get; set; }

        public virtual ICollection<Taxonomy> Taxonomies { get; set; } = new List<Taxonomy>();
    }
}

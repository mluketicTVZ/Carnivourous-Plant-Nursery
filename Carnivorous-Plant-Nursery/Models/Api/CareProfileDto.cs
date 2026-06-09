using System.ComponentModel.DataAnnotations;

namespace Carnivorous_Plant_Nursery.Models.Api
{
    public class CareProfileDto
    {
        public int Id { get; set; }
        public string CareProfileName { get; set; } = string.Empty;
        public LightLevel? RequiredLight { get; set; }
        public TemperatureTolerance? MinTemperature { get; set; }
        public TemperatureTolerance? MaxTemperature { get; set; }
        public string? TemperatureDescription { get; set; }
        public bool? RequiresWinterDormancy { get; set; }
        public string? SoilMix { get; set; }
        public HumidityLevel? RequiredHumidity { get; set; }
        public string? CareDescription { get; set; }
    }

    public class CareProfileSummaryDto
    {
        public int Id { get; set; }
        public string CareProfileName { get; set; } = string.Empty;
    }

    public class CareProfileWriteDto
    {
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
    }
}

using System;

namespace Carnivorous_Plant_Nursery.Models
{
    public class CareProfile
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
}

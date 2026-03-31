using System;

namespace Carnivorous_Plant_Nursery.Models
{
    public class CareProfile
    {
        public int Id { get; set; }
        public string CareProfileName { get; set; }
        public LightLevel RequiredLight { get; set; }
        public decimal? MinTemperatureCelsius { get; set; }
        public decimal? MaxTemperatureCelsius { get; set; }
        public bool RequiresWinterDormancy { get; set; }
        public string SoilMix { get; set; }
        public HumidityLevel RequiredHumidity { get; set; }
    }
}

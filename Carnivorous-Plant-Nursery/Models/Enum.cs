namespace Carnivorous_Plant_Nursery.Models
{
    public enum PlantStage
    {
        Seedling,
        Juvenile,
        Mature,
        Flowering,
        Dormant
    }

    public enum LightLevel
    {
        FullSun,
        PartialShade,
        BrightIndirect,
        LowLight
    }

    public enum HumidityLevel
    {
        Low,        
        Moderate,   
        High,      
        ExtremelyHigh
    }

    public enum HealthState
    {
        Excellent,
        Good,
        Fair,
        Poor,
        Quarantined,
        Dead
    }

    public enum TemperatureTolerance
    {
        // Below 0°C (Risk of frostbite for sensitive plants)
        Freezing,
        
        // 0°C to 10°C (Cold winter weather, often triggers dormancy)
        Cold,
        
        // 10°C to 20°C (Cool spring/autumn weather)
        Cool,
        
        // 20°C to 30°C (Warm, comfortable room temperature, optimal growth)
        Warm,
        
        // 30°C to 40°C (Hot summer days, dew production may struggle on sundews)
        Hot,
        
        // Above 40°C (Scorching extreme heat, highly dangerous without shade/humidity)
        Scorching
    }
}
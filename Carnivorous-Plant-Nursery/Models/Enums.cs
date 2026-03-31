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
}
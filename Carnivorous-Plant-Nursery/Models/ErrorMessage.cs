namespace Carnivorous_Plant_Nursery.Models
{
    public static class ErrorMessage
    {
        public const string NegativeEstimatedAge = "Estimated age can not be negative.";
        public const string NegativeSeedCount = "Seed count can not be negative.";
        public const string InvalidGerminationRate = "Germination rate must be between 0.0 and 1.0.";
    }
}

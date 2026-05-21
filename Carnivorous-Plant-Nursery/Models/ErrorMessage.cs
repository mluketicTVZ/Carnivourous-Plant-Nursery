namespace Carnivorous_Plant_Nursery.Models
{
    public static class ErrorMessage
    {
        public const string NegativeEstimatedAge = "Estimated age can not be negative.";
        public const string NegativeSeedCount = "Seed count can not be negative.";
        public const string InvalidGerminationRate = "Germination rate must be between 0.0 and 1.0.";
        public const string NegativePrice = "Price can not be negative.";
        public const string NegativePotDiameter = "Pot diameter can not be negative.";
        public const string NegativePotHeight = "Pot height can not be negative.";
        public const string DeleteErrorPlantInLineage = "This plant cannot be deleted because it is recorded as a parent in one or more lineage entries. Remove those lineage records first.";
        public const string DeleteErrorSeedBatchInLineage = "This seed batch cannot be deleted because it is recorded as a parent in one or more lineage entries. Remove those lineage records first.";
        public const string DeleteErrorSeedBatchHasPlants = "This seed batch cannot be deleted because one or more plants were sourced from it. Remove or reassign those plant records first.";
        public const string DeleteErrorTaxonomyHasItems = "This taxonomy cannot be deleted because plants or seed batches are assigned to it. Remove or reassign those records first.";
        public const string DeleteErrorCareProfileHasTaxonomies = "This care profile cannot be deleted because one or more taxonomies are assigned to it. Remove or reassign those taxonomies first.";
    }
}

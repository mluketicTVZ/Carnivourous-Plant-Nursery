namespace Carnivorous_Plant_Nursery.Models.Api
{
    public static class ApiMappers
    {
        public static CareProfileDto ToDto(this CareProfile careProfile) =>
            new()
            {
                Id = careProfile.Id,
                CareProfileName = careProfile.CareProfileName,
                RequiredLight = careProfile.RequiredLight,
                MinTemperature = careProfile.MinTemperature,
                MaxTemperature = careProfile.MaxTemperature,
                TemperatureDescription = careProfile.TemperatureDescription,
                RequiresWinterDormancy = careProfile.RequiresWinterDormancy,
                SoilMix = careProfile.SoilMix,
                RequiredHumidity = careProfile.RequiredHumidity,
                CareDescription = careProfile.CareDescription
            };

        public static CareProfileSummaryDto ToSummaryDto(this CareProfile careProfile) =>
            new()
            {
                Id = careProfile.Id,
                CareProfileName = careProfile.CareProfileName
            };

        public static TaxonomyDto ToDto(this Taxonomy taxonomy) =>
            new()
            {
                Id = taxonomy.Id,
                Genus = taxonomy.Genus,
                Species = taxonomy.Species,
                Cultivar = taxonomy.Cultivar,
                CommonName = taxonomy.CommonName,
                FullName = taxonomy.FullName,
                CareProfileId = taxonomy.CareProfileId,
                CareProfile = taxonomy.CareProfile?.ToSummaryDto()
            };

        public static TaxonomySummaryDto ToSummaryDto(this Taxonomy taxonomy) =>
            new()
            {
                Id = taxonomy.Id,
                CommonName = taxonomy.CommonName,
                FullName = taxonomy.FullName
            };

        public static PlantDto ToDto(this Plant plant) =>
            new()
            {
                Id = plant.Id,
                ItemType = nameof(Plant),
                SKU = plant.SKU,
                ListingTitle = plant.ListingTitle,
                Price = plant.Price,
                IsAvailableInWebshop = plant.IsAvailableInWebshop,
                Description = plant.Description,
                TaxonomyId = plant.TaxonomyId,
                Taxonomy = plant.Taxonomy?.ToSummaryDto(),
                LineageId = plant.LineageId,
                DateAcquired = plant.DateAcquired,
                InternalNotes = plant.InternalNotes,
                LocationInNursery = plant.LocationInNursery,
                CurrentStage = plant.CurrentStage,
                PotDiameterCm = plant.PotDiameterCm,
                PotHeightCm = plant.PotHeightCm,
                LastRepottingDate = plant.LastRepottingDate,
                LastDormancyDateStart = plant.LastDormancyDateStart,
                LastDormancyDateEnd = plant.LastDormancyDateEnd,
                EstimatedAgeAtAcquiryYears = plant.EstimatedAgeAtAcquiryYears,
                HealthStatus = plant.HealthStatus,
                HealthDescription = plant.HealthDescription,
                SourceSeedBatchId = plant.SourceSeedBatchId
            };

        public static SeedBatchDto ToDto(this SeedBatch seedBatch) =>
            new()
            {
                Id = seedBatch.Id,
                ItemType = nameof(SeedBatch),
                SKU = seedBatch.SKU,
                ListingTitle = seedBatch.ListingTitle,
                Price = seedBatch.Price,
                IsAvailableInWebshop = seedBatch.IsAvailableInWebshop,
                Description = seedBatch.Description,
                TaxonomyId = seedBatch.TaxonomyId,
                Taxonomy = seedBatch.Taxonomy?.ToSummaryDto(),
                LineageId = seedBatch.LineageId,
                DateAcquired = seedBatch.DateAcquired,
                InternalNotes = seedBatch.InternalNotes,
                LocationInNursery = seedBatch.LocationInNursery,
                SeedCount = seedBatch.SeedCount,
                HarvestDate = seedBatch.HarvestDate,
                ExpectedViabilityMonths = seedBatch.ExpectedViabilityMonths,
                RequiresStratification = seedBatch.RequiresStratification,
                EstimatedGerminationRate = seedBatch.EstimatedGerminationRate
            };

        public static InventoryItemSummaryDto ToSummaryDto(this InventoryItem item) =>
            new()
            {
                Id = item.Id,
                ItemType = item.GetType().Name,
                SKU = item.SKU,
                ListingTitle = item.ListingTitle,
                Price = item.Price,
                IsAvailableInWebshop = item.IsAvailableInWebshop,
                TaxonomyId = item.TaxonomyId,
                Taxonomy = item.Taxonomy?.ToSummaryDto()
            };

        public static void ApplyTo(this CareProfileWriteDto dto, CareProfile careProfile)
        {
            careProfile.CareProfileName = dto.CareProfileName;
            careProfile.RequiredLight = dto.RequiredLight;
            careProfile.MinTemperature = dto.MinTemperature;
            careProfile.MaxTemperature = dto.MaxTemperature;
            careProfile.TemperatureDescription = dto.TemperatureDescription;
            careProfile.RequiresWinterDormancy = dto.RequiresWinterDormancy;
            careProfile.SoilMix = dto.SoilMix;
            careProfile.RequiredHumidity = dto.RequiredHumidity;
            careProfile.CareDescription = dto.CareDescription;
        }

        public static void ApplyTo(this TaxonomyWriteDto dto, Taxonomy taxonomy)
        {
            taxonomy.Genus = dto.Genus;
            taxonomy.Species = dto.Species;
            taxonomy.Cultivar = dto.Cultivar;
            taxonomy.CommonName = dto.CommonName;
            taxonomy.CareProfileId = dto.CareProfileId;
        }

        public static void ApplyCommonTo(this InventoryItemWriteDto dto, InventoryItem item)
        {
            item.SKU = dto.SKU;
            item.ListingTitle = dto.ListingTitle;
            item.Price = dto.Price;
            item.IsAvailableInWebshop = dto.IsAvailableInWebshop;
            item.Description = dto.Description;
            item.TaxonomyId = dto.TaxonomyId;
            item.LineageId = dto.LineageId;
            item.DateAcquired = dto.DateAcquired;
            item.InternalNotes = dto.InternalNotes;
            item.LocationInNursery = dto.LocationInNursery;
        }

        public static void ApplyTo(this PlantWriteDto dto, Plant plant)
        {
            dto.ApplyCommonTo(plant);
            plant.CurrentStage = dto.CurrentStage;
            plant.PotDiameterCm = dto.PotDiameterCm;
            plant.PotHeightCm = dto.PotHeightCm;
            plant.LastRepottingDate = dto.LastRepottingDate;
            plant.LastDormancyDateStart = dto.LastDormancyDateStart;
            plant.LastDormancyDateEnd = dto.LastDormancyDateEnd;
            plant.EstimatedAgeAtAcquiryYears = dto.EstimatedAgeAtAcquiryYears;
            plant.HealthStatus = dto.HealthStatus;
            plant.HealthDescription = dto.HealthDescription;
            plant.SourceSeedBatchId = dto.SourceSeedBatchId;
        }

        public static void ApplyTo(this SeedBatchWriteDto dto, SeedBatch seedBatch)
        {
            dto.ApplyCommonTo(seedBatch);
            seedBatch.SeedCount = dto.SeedCount;
            seedBatch.HarvestDate = dto.HarvestDate;
            seedBatch.ExpectedViabilityMonths = dto.ExpectedViabilityMonths;
            seedBatch.RequiresStratification = dto.RequiresStratification;
            seedBatch.EstimatedGerminationRate = dto.EstimatedGerminationRate;
        }
    }
}

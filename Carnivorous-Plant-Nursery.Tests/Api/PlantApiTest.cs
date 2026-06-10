using System.Net;
using System.Net.Http.Json;
using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Tests.Api
{
    public class PlantApiTest : IClassFixture<NurseryWebApplicationFactory>
    {
        private readonly NurseryWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public PlantApiTest(NurseryWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnMatchingPlants_WhenFiltersProvided()
        {
            var matchingPlant = new Plant
            {
                ListingTitle = "Integration Test Filtered Plant",
                SKU = "PLANT-FILTER-001",
                IsAvailableInWebshop = true,
                CurrentStage = PlantStage.Mature,
                HealthStatus = HealthState.Good
            };
            var hiddenPlant = new Plant
            {
                ListingTitle = "Integration Test Hidden Plant",
                SKU = "PLANT-HIDDEN-001",
                IsAvailableInWebshop = false,
                CurrentStage = PlantStage.Seedling,
                HealthStatus = HealthState.Poor
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Plant.AddRange(matchingPlant, hiddenPlant);
                await dbContext.SaveChangesAsync();
            });

            var response = await _client.GetAsync("/api/plant?searchTerm=Filtered&webshopOnly=true&stage=Mature&healthStatus=Good");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var plants = await response.Content.ReadFromJsonAsync<List<PlantDto>>();
            plants.Should().NotBeNull();
            plants!.Should().Contain(p => p.Id == matchingPlant.Id);
            plants.Should().NotContain(p => p.Id == hiddenPlant.Id);
        }

        [Fact]
        public async Task Create_ShouldPersistPlant_WhenUserIsManager()
        {
            var taxonomy = new Taxonomy
            {
                Genus = "Drosera",
                Species = "binata",
                CommonName = "Integration Forked Sundew"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Taxonomy.Add(taxonomy);
                await dbContext.SaveChangesAsync();
            });

            var request = new PlantWriteDto
            {
                SKU = "PLANT-CREATE-001",
                ListingTitle = "Integration Test Created Plant",
                Price = 24.50m,
                IsAvailableInWebshop = true,
                Description = "Created through API integration test.",
                TaxonomyId = taxonomy.Id,
                LocationInNursery = "Bench A",
                CurrentStage = PlantStage.Juvenile,
                PotDiameterCm = 8.5m,
                PotHeightCm = 10m,
                EstimatedAgeAtAcquiryYears = 2,
                HealthStatus = HealthState.Excellent
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/plant")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Manager);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<PlantDto>();
            created.Should().NotBeNull();
            created!.ListingTitle.Should().Be(request.ListingTitle);
            created.TaxonomyId.Should().Be(taxonomy.Id);
            created.CurrentStage.Should().Be(request.CurrentStage);

            var persisted = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.Plant.SingleAsync(p => p.Id == created.Id));

            persisted.SKU.Should().Be(request.SKU);
            persisted.DeletedAt.Should().BeNull();
        }

        [Fact]
        public async Task Update_ShouldPersistPlant_WhenUserIsManager()
        {
            var plant = new Plant
            {
                SKU = "PLANT-UPDATE-001",
                ListingTitle = "Integration Plant Before Update",
                Price = 12m,
                CurrentStage = PlantStage.Seedling,
                HealthStatus = HealthState.Fair
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Plant.Add(plant);
                await dbContext.SaveChangesAsync();
            });

            var request = new PlantWriteDto
            {
                SKU = "PLANT-UPDATE-002",
                ListingTitle = "Integration Plant After Update",
                Price = 18m,
                IsAvailableInWebshop = true,
                CurrentStage = PlantStage.Mature,
                HealthStatus = HealthState.Good,
                HealthDescription = "Recovered after repotting."
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/plant/{plant.Id}")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Manager);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await response.Content.ReadFromJsonAsync<PlantDto>();
            updated.Should().NotBeNull();
            updated!.ListingTitle.Should().Be(request.ListingTitle);
            updated.CurrentStage.Should().Be(request.CurrentStage);

            var persisted = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.Plant.SingleAsync(p => p.Id == plant.Id));

            persisted.SKU.Should().Be(request.SKU);
            persisted.HealthDescription.Should().Be(request.HealthDescription);
        }

        [Fact]
        public async Task Delete_ShouldSoftDeletePlant_WhenUserIsAdmin()
        {
            var plant = new Plant
            {
                ListingTitle = "Integration Plant For Delete"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Plant.Add(plant);
                await dbContext.SaveChangesAsync();
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/plant/{plant.Id}");
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Admin);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedAt = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.Plant
                    .Where(p => p.Id == plant.Id)
                    .Select(p => p.DeletedAt)
                    .SingleAsync());

            deletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldReturnConflict_WhenPlantIsUsedInLineage()
        {
            var plant = new Plant
            {
                ListingTitle = "Integration Parent Plant"
            };
            var lineage = new Lineage
            {
                Mother = plant,
                Generation = "Integration F1"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Lineage.Add(lineage);
                await dbContext.SaveChangesAsync();
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/plant/{plant.Id}");
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Admin);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}

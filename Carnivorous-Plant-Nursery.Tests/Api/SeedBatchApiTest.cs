using System.Net;
using System.Net.Http.Json;
using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Tests.Api
{
    public class SeedBatchApiTest : IClassFixture<NurseryWebApplicationFactory>
    {
        private readonly NurseryWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public SeedBatchApiTest(NurseryWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnMatchingSeedBatches_WhenFiltersProvided()
        {
            var matchingSeedBatch = new SeedBatch
            {
                ListingTitle = "Integration Test Filtered Seeds",
                SKU = "SEED-FILTER-001",
                IsAvailableInWebshop = true,
                SeedCount = 30
            };
            var hiddenSeedBatch = new SeedBatch
            {
                ListingTitle = "Integration Test Hidden Seeds",
                SKU = "SEED-HIDDEN-001",
                IsAvailableInWebshop = false,
                SeedCount = 50
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.SeedBatch.AddRange(matchingSeedBatch, hiddenSeedBatch);
                await dbContext.SaveChangesAsync();
            });

            var response = await _client.GetAsync("/api/seeds?searchTerm=Filtered&availableInWebshop=true");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var seedBatches = await response.Content.ReadFromJsonAsync<List<SeedBatchDto>>();
            seedBatches.Should().NotBeNull();
            seedBatches!.Should().Contain(s => s.Id == matchingSeedBatch.Id);
            seedBatches.Should().NotContain(s => s.Id == hiddenSeedBatch.Id);
        }

        [Fact]
        public async Task Create_ShouldPersistSeedBatch_WhenUserIsManager()
        {
            var taxonomy = new Taxonomy
            {
                Genus = "Sarracenia",
                Species = "leucophylla",
                CommonName = "Integration White Pitcher Seeds"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Taxonomy.Add(taxonomy);
                await dbContext.SaveChangesAsync();
            });

            var request = new SeedBatchWriteDto
            {
                SKU = "SEED-CREATE-001",
                ListingTitle = "Integration Test Created Seeds",
                Price = 7.50m,
                IsAvailableInWebshop = true,
                Description = "Created through API integration test.",
                TaxonomyId = taxonomy.Id,
                SeedCount = 120,
                HarvestDate = new DateTime(2026, 1, 15),
                ExpectedViabilityMonths = 18,
                RequiresStratification = true,
                EstimatedGerminationRate = 0.85m
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/seeds")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Manager);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<SeedBatchDto>();
            created.Should().NotBeNull();
            created!.ListingTitle.Should().Be(request.ListingTitle);
            created.TaxonomyId.Should().Be(taxonomy.Id);
            created.SeedCount.Should().Be(request.SeedCount);

            var persisted = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.SeedBatch.SingleAsync(s => s.Id == created.Id));

            persisted.SKU.Should().Be(request.SKU);
            persisted.DeletedAt.Should().BeNull();
        }

        [Fact]
        public async Task Update_ShouldPersistSeedBatch_WhenUserIsManager()
        {
            var seedBatch = new SeedBatch
            {
                SKU = "SEED-UPDATE-001",
                ListingTitle = "Integration Seeds Before Update",
                SeedCount = 20,
                EstimatedGerminationRate = 0.40m
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.SeedBatch.Add(seedBatch);
                await dbContext.SaveChangesAsync();
            });

            var request = new SeedBatchWriteDto
            {
                SKU = "SEED-UPDATE-002",
                ListingTitle = "Integration Seeds After Update",
                Price = 9m,
                IsAvailableInWebshop = true,
                SeedCount = 45,
                RequiresStratification = false,
                EstimatedGerminationRate = 0.70m
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/seeds/{seedBatch.Id}")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Manager);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await response.Content.ReadFromJsonAsync<SeedBatchDto>();
            updated.Should().NotBeNull();
            updated!.ListingTitle.Should().Be(request.ListingTitle);
            updated.SeedCount.Should().Be(request.SeedCount);

            var persisted = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.SeedBatch.SingleAsync(s => s.Id == seedBatch.Id));

            persisted.SKU.Should().Be(request.SKU);
            persisted.EstimatedGerminationRate.Should().Be(request.EstimatedGerminationRate);
        }

        [Fact]
        public async Task Delete_ShouldSoftDeleteSeedBatch_WhenUserIsAdmin()
        {
            var seedBatch = new SeedBatch
            {
                ListingTitle = "Integration Seeds For Delete"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.SeedBatch.Add(seedBatch);
                await dbContext.SaveChangesAsync();
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/seeds/{seedBatch.Id}");
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Admin);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedAt = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.SeedBatch
                    .Where(s => s.Id == seedBatch.Id)
                    .Select(s => s.DeletedAt)
                    .SingleAsync());

            deletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldReturnConflict_WhenSeedBatchIsPlantSource()
        {
            var seedBatch = new SeedBatch
            {
                ListingTitle = "Integration Source Seeds"
            };
            var plant = new Plant
            {
                ListingTitle = "Integration Plant From Seeds",
                SourceSeedBatch = seedBatch
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Plant.Add(plant);
                await dbContext.SaveChangesAsync();
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/seeds/{seedBatch.Id}");
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Admin);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}

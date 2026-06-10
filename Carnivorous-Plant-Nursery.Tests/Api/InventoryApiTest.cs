using System.Net;
using System.Net.Http.Json;
using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Tests.Api
{
    public class InventoryApiTest : IClassFixture<NurseryWebApplicationFactory>
    {
        private readonly NurseryWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public InventoryApiTest(NurseryWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnPlantsAndSeedBatches_WhenSearchTermProvided()
        {
            var plant = new Plant
            {
                ListingTitle = "Integration Inventory Shared Plant",
                SKU = "INV-SHARED-PLANT",
                IsAvailableInWebshop = true
            };
            var seedBatch = new SeedBatch
            {
                ListingTitle = "Integration Inventory Shared Seeds",
                SKU = "INV-SHARED-SEED",
                IsAvailableInWebshop = true
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Plant.Add(plant);
                dbContext.SeedBatch.Add(seedBatch);
                await dbContext.SaveChangesAsync();
            });

            var response = await _client.GetAsync("/api/inventory?searchTerm=Integration%20Inventory%20Shared&webshopOnly=true");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var items = await response.Content.ReadFromJsonAsync<List<InventoryItemSummaryDto>>();
            items.Should().NotBeNull();
            items!.Should().Contain(i => i.Id == plant.Id && i.ItemType == nameof(Plant));
            items.Should().Contain(i => i.Id == seedBatch.Id && i.ItemType == nameof(SeedBatch));
        }

        [Fact]
        public async Task GetById_ShouldReturnPlant_WhenPlantExists()
        {
            var plant = new Plant
            {
                ListingTitle = "Integration Inventory Plant By Id",
                SKU = "INV-PLANT-ID"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Plant.Add(plant);
                await dbContext.SaveChangesAsync();
            });

            var response = await _client.GetAsync($"/api/inventory/{plant.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var item = await response.Content.ReadFromJsonAsync<InventoryItemSummaryDto>();
            item.Should().NotBeNull();
            item!.Id.Should().Be(plant.Id);
            item.ItemType.Should().Be(nameof(Plant));
        }

        [Fact]
        public async Task GetById_ShouldReturnSeedBatch_WhenSeedBatchExists()
        {
            var seedBatch = new SeedBatch
            {
                ListingTitle = "Integration Inventory Seed By Id",
                SKU = "INV-SEED-ID"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.SeedBatch.Add(seedBatch);
                await dbContext.SaveChangesAsync();
            });

            var response = await _client.GetAsync($"/api/inventory/{seedBatch.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var item = await response.Content.ReadFromJsonAsync<InventoryItemSummaryDto>();
            item.Should().NotBeNull();
            item!.Id.Should().Be(seedBatch.Id);
            item.ItemType.Should().Be(nameof(SeedBatch));
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenInventoryItemDoesNotExist()
        {
            var maxId = await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                var plantMax = await dbContext.Plant.Select(p => (int?)p.Id).MaxAsync() ?? 0;
                var seedMax = await dbContext.SeedBatch.Select(s => (int?)s.Id).MaxAsync() ?? 0;
                return Math.Max(plantMax, seedMax);
            });

            var response = await _client.GetAsync($"/api/inventory/{maxId + 10_000}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

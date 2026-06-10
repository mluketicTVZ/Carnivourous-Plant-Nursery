using System.Net;
using System.Net.Http.Json;
using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Tests.Api
{
    public class TaxonomyApiTest : IClassFixture<NurseryWebApplicationFactory>
    {
        private readonly NurseryWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TaxonomyApiTest(NurseryWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnMatchingTaxonomies_WhenSearchTermProvided()
        {
            var taxonomy = new Taxonomy
            {
                Genus = "Integrationus",
                Species = "searchensis",
                CommonName = "Integration Search Sundew"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Taxonomy.Add(taxonomy);
                await dbContext.SaveChangesAsync();
            });

            var response = await _client.GetAsync("/api/taxonomy?searchTerm=Integration%20Search");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var taxonomies = await response.Content.ReadFromJsonAsync<List<TaxonomyDto>>();
            taxonomies.Should().NotBeNull();
            taxonomies!.Should().Contain(t => t.Id == taxonomy.Id);
        }

        [Fact]
        public async Task Create_ShouldPersistTaxonomy_WhenUserIsManager()
        {
            var careProfile = new CareProfile
            {
                CareProfileName = "Integration Test Taxonomy Care"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.CareProfile.Add(careProfile);
                await dbContext.SaveChangesAsync();
            });

            var request = new TaxonomyWriteDto
            {
                Genus = "Sarracenia",
                Species = "flava",
                Cultivar = "Integration Gold",
                CommonName = "Integration Pitcher",
                CareProfileId = careProfile.Id
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/taxonomy")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Manager);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<TaxonomyDto>();
            created.Should().NotBeNull();
            created!.Genus.Should().Be(request.Genus);
            created.CareProfileId.Should().Be(careProfile.Id);
            created.CareProfile.Should().NotBeNull();

            var persisted = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.Taxonomy.SingleAsync(t => t.Id == created.Id));

            persisted.CommonName.Should().Be(request.CommonName);
            persisted.DeletedAt.Should().BeNull();
        }

        [Fact]
        public async Task Update_ShouldPersistTaxonomy_WhenUserIsManager()
        {
            var taxonomy = new Taxonomy
            {
                Genus = "Dionaea",
                Species = "muscipula",
                CommonName = "Integration Flytrap Before"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Taxonomy.Add(taxonomy);
                await dbContext.SaveChangesAsync();
            });

            var request = new TaxonomyWriteDto
            {
                Genus = "Dionaea",
                Species = "muscipula",
                Cultivar = "Integration Snap",
                CommonName = "Integration Flytrap After"
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/taxonomy/{taxonomy.Id}")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Manager);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await response.Content.ReadFromJsonAsync<TaxonomyDto>();
            updated.Should().NotBeNull();
            updated!.CommonName.Should().Be(request.CommonName);
            updated.Cultivar.Should().Be(request.Cultivar);

            var persisted = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.Taxonomy.SingleAsync(t => t.Id == taxonomy.Id));

            persisted.CommonName.Should().Be(request.CommonName);
            persisted.Cultivar.Should().Be(request.Cultivar);
        }

        [Fact]
        public async Task Delete_ShouldSoftDeleteTaxonomy_WhenUserIsAdmin()
        {
            var taxonomy = new Taxonomy
            {
                Genus = "Pinguicula",
                Species = "vulgaris",
                CommonName = "Integration Butterwort Delete"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Taxonomy.Add(taxonomy);
                await dbContext.SaveChangesAsync();
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/taxonomy/{taxonomy.Id}");
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Admin);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedAt = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.Taxonomy
                    .Where(t => t.Id == taxonomy.Id)
                    .Select(t => t.DeletedAt)
                    .SingleAsync());

            deletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldReturnConflict_WhenTaxonomyHasInventoryItems()
        {
            var taxonomy = new Taxonomy
            {
                Genus = "Nepenthes",
                Species = "ventricosa",
                CommonName = "Integration Referenced Taxonomy"
            };
            var plant = new Plant
            {
                ListingTitle = "Integration Referencing Plant",
                Taxonomy = taxonomy
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Plant.Add(plant);
                await dbContext.SaveChangesAsync();
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/taxonomy/{taxonomy.Id}");
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Admin);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}

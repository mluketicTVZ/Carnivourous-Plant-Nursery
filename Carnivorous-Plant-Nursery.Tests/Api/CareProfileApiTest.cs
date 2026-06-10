using System.Net;
using System.Net.Http.Json;
using Carnivorous_Plant_Nursery.Models;
using Carnivorous_Plant_Nursery.Models.Api;
using Carnivorous_Plant_Nursery.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Tests.Api
{
    public class CareProfileApiTest : IClassFixture<NurseryWebApplicationFactory>
    {
        private readonly NurseryWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CareProfileApiTest(NurseryWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnMatchingCareProfiles_WhenSearchTermProvided()
        {
            var careProfile = new CareProfile
            {
                CareProfileName = "Integration Test Searchable Mist Care",
                RequiredLight = LightLevel.LowLight,
                RequiredHumidity = HumidityLevel.ExtremelyHigh
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.CareProfile.Add(careProfile);
                await dbContext.SaveChangesAsync();
            });

            var response = await _client.GetAsync("/api/care?searchTerm=Searchable%20Mist&requiredLight=LowLight");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var careProfiles = await response.Content.ReadFromJsonAsync<List<CareProfileDto>>();
            careProfiles.Should().NotBeNull();
            careProfiles!.Should().Contain(c => c.Id == careProfile.Id);
            careProfiles.Should().OnlyContain(c => c.RequiredLight == LightLevel.LowLight);
        }

        [Fact]
        public async Task Create_ShouldPersistCareProfile_WhenUserIsManager()
        {
            var request = new CareProfileWriteDto
            {
                CareProfileName = "Integration Test Highland Care",
                RequiredLight = LightLevel.BrightIndirect,
                MinTemperature = TemperatureTolerance.Cool,
                MaxTemperature = TemperatureTolerance.Warm,
                TemperatureDescription = "Cool nights and gentle days.",
                RequiresWinterDormancy = true,
                SoilMix = "Live sphagnum and perlite",
                RequiredHumidity = HumidityLevel.High,
                CareDescription = "Keep consistently moist with good airflow."
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/care")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Manager);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var created = await response.Content.ReadFromJsonAsync<CareProfileDto>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);
            created.CareProfileName.Should().Be(request.CareProfileName);
            created.RequiredLight.Should().Be(request.RequiredLight);
            created.RequiredHumidity.Should().Be(request.RequiredHumidity);

            var persisted = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.CareProfile.SingleAsync(c => c.Id == created.Id));

            persisted.CareProfileName.Should().Be(request.CareProfileName);
            persisted.DeletedAt.Should().BeNull();
            persisted.SoilMix.Should().Be(request.SoilMix);
        }

        [Fact]
        public async Task Create_ShouldRejectCareProfile_WhenUserIsCustomer()
        {
            var request = new CareProfileWriteDto
            {
                CareProfileName = "Integration Test Customer Care"
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/care")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Customer);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_ShouldPersistCareProfile_WhenUserIsManager()
        {
            var careProfile = new CareProfile
            {
                CareProfileName = "Integration Test Care Before Update",
                RequiredLight = LightLevel.FullSun
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.CareProfile.Add(careProfile);
                await dbContext.SaveChangesAsync();
            });

            var request = new CareProfileWriteDto
            {
                CareProfileName = "Integration Test Care After Update",
                RequiredLight = LightLevel.BrightIndirect,
                SoilMix = "Sand and peat"
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/care/{careProfile.Id}")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Manager);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await response.Content.ReadFromJsonAsync<CareProfileDto>();
            updated.Should().NotBeNull();
            updated!.CareProfileName.Should().Be(request.CareProfileName);
            updated.RequiredLight.Should().Be(request.RequiredLight);

            var persisted = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.CareProfile.SingleAsync(c => c.Id == careProfile.Id));

            persisted.CareProfileName.Should().Be(request.CareProfileName);
            persisted.SoilMix.Should().Be(request.SoilMix);
        }

        [Fact]
        public async Task Delete_ShouldSoftDeleteCareProfile_WhenUserIsAdmin()
        {
            var careProfile = new CareProfile
            {
                CareProfileName = "Integration Test Care For Delete"
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.CareProfile.Add(careProfile);
                await dbContext.SaveChangesAsync();
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/care/{careProfile.Id}");
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Admin);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedAt = await _factory.ExecuteDbContextAsync(dbContext =>
                dbContext.CareProfile
                    .Where(c => c.Id == careProfile.Id)
                    .Select(c => c.DeletedAt)
                    .SingleAsync());

            deletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldReturnConflict_WhenCareProfileHasTaxonomies()
        {
            var careProfile = new CareProfile
            {
                CareProfileName = "Integration Test Referenced Care"
            };
            var taxonomy = new Taxonomy
            {
                Genus = "Drosera",
                Species = "capensis",
                CareProfile = careProfile
            };

            await _factory.ExecuteDbContextAsync(async dbContext =>
            {
                dbContext.Taxonomy.Add(taxonomy);
                await dbContext.SaveChangesAsync();
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/care/{careProfile.Id}");
            httpRequest.Headers.Add(TestAuthHandler.RoleHeaderName, AuthorizationRole.Admin);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}

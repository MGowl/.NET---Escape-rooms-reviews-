using System.Net;
using System.Net.Http.Json;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EscapeRoomReviews.Tests.Api;

public class LocationApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public LocationApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static async Task SeedLocationAsync(ApplicationDbContext db)
    {
        db.Locations.Add(new Location { Id = 100, City = "Test City", Address = "Test Address 1", PostalCode = "10000" });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkAndList_WhenLocationsExist()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedLocationAsync(db);

        var response = await _client.GetAsync("/api/location");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>();
        list.Should().NotBeNull();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenLocationExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedLocationAsync(db);

        var response = await client.GetAsync("/api/location/100");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<LocationDTO>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(100);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenLocationNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/location/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201_WhenModelIsValid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Locations.RemoveRange(await db.Locations.ToListAsync());
        await db.SaveChangesAsync();

        var requestBody = new { City = "Novi Grad", Address = "Nova ulica 1", PostalCode = "20000", Latitude = 44.0, Longitude = 16.0 };
        var response = await client.PostAsJsonAsync("/api/location", requestBody);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenModelIsInvalid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/location", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200_WhenLocationExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedLocationAsync(db);

        var updateBody = new { City = "Ažurirani Grad", Address = "Ažurirana ulica 10", PostalCode = "30000", Latitude = 45.0, Longitude = 17.0 };
        var response = await client.PutAsJsonAsync("/api/location/100", updateBody);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<LocationDTO>();
        dto!.City.Should().Be("Ažurirani Grad");
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenLocationNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var updateBody = new { City = "Ažurirani Grad", Address = "Ažurirana ulica 10", PostalCode = "", Latitude = 0.0, Longitude = 0.0 };
        var response = await client.PutAsJsonAsync("/api/location/99999", updateBody);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenLocationExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedLocationAsync(db);

        var response = await client.DeleteAsync("/api/location/100");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenLocationNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/location/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

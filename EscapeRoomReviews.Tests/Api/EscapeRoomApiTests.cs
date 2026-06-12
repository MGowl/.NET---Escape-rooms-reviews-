using System.Net;
using System.Net.Http.Json;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EscapeRoomReviews.Tests.Api;

public class EscapeRoomApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public EscapeRoomApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static async Task SeedEscapeRoomAsync(ApplicationDbContext db)
    {
        var location = new Location
        {
            Id = 100,
            City = "Test City",
            Address = "Test Address 1",
            PostalCode = "10000"
        };

        var company = new Company
        {
            Id = 100,
            Name = "Test Company"
        };

        db.Locations.Add(location);
        db.Companies.Add(company);
        await db.SaveChangesAsync();

        var room = new EscapeRoom
        {
            Id = 100,
            Name = "Test Escape Room",
            Description = "Test description",
            Difficulty = DifficultyLevel.Easy,
            MaxPlayers = 4,
            Price = 50m,
            CreatedAt = DateTime.UtcNow,
            LocationId = location.Id,
            CompanyId = company.Id
        };

        db.EscapeRooms.Add(room);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkAndList_WhenRoomsExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedEscapeRoomAsync(db);

        // Act
        var response = await _client.GetAsync("/api/escaperoom");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.Content.ReadFromJsonAsync<List<object>>();
        list.Should().NotBeNull();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenRoomExists()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedEscapeRoomAsync(db);

        // Act
        var response = await client.GetAsync("/api/escaperoom/100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<EscapeRoomDTO>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(100);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenRoomNotExists()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/escaperoom/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201_WhenModelIsValid()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // The DbSeeder adds rooms with explicit IDs 1–3 on startup.
        // The InMemory integer generator starts at 0 (first generated value = 1),
        // so we must remove those rows before POST to avoid a duplicate-key conflict.
        db.EscapeRooms.RemoveRange(await db.EscapeRooms.ToListAsync());
        await db.SaveChangesAsync();

        await SeedEscapeRoomAsync(db); // adds Location(100), Company(100), EscapeRoom(100)

        var requestBody = new
        {
            Name = "Nova soba",
            Description = "Opis sobe",
            Difficulty = 2,
            MaxPlayers = 4,
            Price = 60.0,
            LocationId = 100,
            CompanyId = 100
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/escaperoom", requestBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenModelIsInvalid()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Act — empty object fails Required/Range validations on Name, MaxPlayers, Price, LocationId, CompanyId
        var response = await client.PostAsJsonAsync("/api/escaperoom", new { });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200_WhenRoomExists()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedEscapeRoomAsync(db);

        var updateBody = new
        {
            Name = "Ažurirana soba",
            Description = "Novi opis",
            Difficulty = 3,
            MaxPlayers = 5,
            Price = 75.0,
            LocationId = 100,
            CompanyId = 100
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/escaperoom/100", updateBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<EscapeRoomDTO>();
        dto!.Name.Should().Be("Ažurirana soba");
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenRoomNotExists()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // LocationId/CompanyId 1 exist from DbSeeder; model is valid so the controller
        // reaches the room-lookup step and returns 404 for the missing room.
        var updateBody = new
        {
            Name = "Ažurirana soba",
            Description = "",
            Difficulty = 2,
            MaxPlayers = 4,
            Price = 50.0,
            LocationId = 1,
            CompanyId = 1
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/escaperoom/99999", updateBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenRoomExists()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedEscapeRoomAsync(db);

        // Act
        var response = await client.DeleteAsync("/api/escaperoom/100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenRoomNotExists()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/escaperoom/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

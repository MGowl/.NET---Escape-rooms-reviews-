using System.Net;
using System.Net.Http.Json;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EscapeRoomReviews.Tests.Api;

public class ReviewApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ReviewApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Seeds a self-contained review with all required dependencies at Id=100.
    private static async Task SeedReviewAsync(ApplicationDbContext db)
    {
        var location = new Location { Id = 100, City = "Test City", Address = "Test Address 1", PostalCode = "10000" };
        var company = new Company { Id = 100, Name = "Test Company" };
        db.Locations.Add(location);
        db.Companies.Add(company);
        await db.SaveChangesAsync();

        var escapeRoom = new EscapeRoom
        {
            Id = 100, Name = "Test Escape Room", Description = "Test description",
            Difficulty = DifficultyLevel.Easy, MaxPlayers = 4, Price = 50m,
            CreatedAt = DateTime.UtcNow, LocationId = 100, CompanyId = 100
        };
        db.EscapeRooms.Add(escapeRoom);

        // AppUsers DbSet maps to the domain User entity (Set<User>()).
        var user = new User { Id = 100, Username = "testreviewer", Email = "reviewer@test.com", RegisteredAt = DateTime.UtcNow };
        db.AppUsers.Add(user);
        await db.SaveChangesAsync();

        var review = new Review
        {
            Id = 100, Rating = 4, Comment = "Odlican escape room svakako preporucujem",
            PlayersCount = 2, CreatedAt = DateTime.UtcNow, EscapeRoomId = 100, UserId = 100
        };
        db.Reviews.Add(review);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkAndList_WhenReviewsExist()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedReviewAsync(db);

        var response = await _client.GetAsync("/api/review");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>();
        list.Should().NotBeNull();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenReviewExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedReviewAsync(db);

        var response = await client.GetAsync("/api/review/100");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ReviewDTO>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(100);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenReviewNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/review/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201_WhenModelIsValid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clear reviews so InMemory ID generator (starts at 1) doesn't conflict
        // with the seeder's explicit review IDs 1–6.
        db.Reviews.RemoveRange(await db.Reviews.ToListAsync());
        await db.SaveChangesAsync();

        // EscapeRoom(Id=1) and User(Id=1) exist from DbSeeder.
        var requestBody = new
        {
            Rating = 4,
            Comment = "Odlican escape room svakako preporucujem",
            PlayersCount = 3,
            EscapeRoomId = 1,
            UserId = 1
        };
        var response = await client.PostAsJsonAsync("/api/review", requestBody);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenModelIsInvalid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // Empty object fails: Rating (Range 1–5), Comment (Required/MinLength 5),
        // PlayersCount (Range >=1), EscapeRoomId (Range >=1), UserId (Range >=1).
        var response = await client.PostAsJsonAsync("/api/review", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200_WhenReviewExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedReviewAsync(db);

        var updateBody = new
        {
            Rating = 5,
            Comment = "Ažurirani komentar — izvrsna soba, topla preporuka",
            PlayersCount = 4,
            EscapeRoomId = 100,
            UserId = 100
        };
        var response = await client.PutAsJsonAsync("/api/review/100", updateBody);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ReviewDTO>();
        dto!.Rating.Should().Be(5);
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenReviewNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // UserId and EscapeRoomId use seeder IDs (1) just to pass Range validation.
        // PUT does not verify their existence in the DB.
        var updateBody = new
        {
            Rating = 4,
            Comment = "Ažurirani komentar — soba nije pronađena testni slučaj",
            PlayersCount = 3,
            EscapeRoomId = 1,
            UserId = 1
        };
        var response = await client.PutAsJsonAsync("/api/review/99999", updateBody);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenReviewExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedReviewAsync(db);

        var response = await client.DeleteAsync("/api/review/100");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenReviewNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/review/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

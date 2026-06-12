using System.Net;
using System.Net.Http.Json;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EscapeRoomReviews.Tests.Api;

public class ThemeApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ThemeApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static async Task SeedThemeAsync(ApplicationDbContext db)
    {
        db.Themes.Add(new Theme { Id = 100, Name = "Test Theme", IconUrl = "/icons/test.svg" });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkAndList_WhenThemesExist()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedThemeAsync(db);

        var response = await _client.GetAsync("/api/theme");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>();
        list.Should().NotBeNull();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenThemeExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedThemeAsync(db);

        var response = await client.GetAsync("/api/theme/100");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ThemeDTO>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(100);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenThemeNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/theme/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201_WhenModelIsValid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Themes.RemoveRange(await db.Themes.ToListAsync());
        await db.SaveChangesAsync();

        var requestBody = new { Name = "Nova tema", IconUrl = "/icons/nova.svg" };
        var response = await client.PostAsJsonAsync("/api/theme", requestBody);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenModelIsInvalid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/theme", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200_WhenThemeExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedThemeAsync(db);

        var updateBody = new { Name = "Ažurirana tema", IconUrl = "/icons/azurirana.svg" };
        var response = await client.PutAsJsonAsync("/api/theme/100", updateBody);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ThemeDTO>();
        dto!.Name.Should().Be("Ažurirana tema");
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenThemeNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var updateBody = new { Name = "Ažurirana tema", IconUrl = "" };
        var response = await client.PutAsJsonAsync("/api/theme/99999", updateBody);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenThemeExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedThemeAsync(db);

        var response = await client.DeleteAsync("/api/theme/100");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenThemeNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/theme/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

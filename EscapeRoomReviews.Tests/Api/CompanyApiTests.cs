using System.Net;
using System.Net.Http.Json;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EscapeRoomReviews.Tests.Api;

public class CompanyApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CompanyApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static async Task SeedCompanyAsync(ApplicationDbContext db)
    {
        db.Companies.Add(new Company { Id = 100, Name = "Test Company", Website = "https://test.hr" });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkAndList_WhenCompaniesExist()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedCompanyAsync(db);

        var response = await _client.GetAsync("/api/company");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<object>>();
        list.Should().NotBeNull();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenCompanyExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedCompanyAsync(db);

        var response = await client.GetAsync("/api/company/100");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(100);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenCompanyNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/company/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn201_WhenModelIsValid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Companies.RemoveRange(await db.Companies.ToListAsync());
        await db.SaveChangesAsync();

        var requestBody = new { Name = "Nova kompanija", Website = "https://nova.hr" };
        var response = await client.PostAsJsonAsync("/api/company", requestBody);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ShouldReturn400_WhenModelIsInvalid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/company", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn200_WhenCompanyExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedCompanyAsync(db);

        var updateBody = new { Name = "Ažurirana kompanija", Website = "https://azurirana.hr" };
        var response = await client.PutAsJsonAsync("/api/company/100", updateBody);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        dto!.Name.Should().Be("Ažurirana kompanija");
    }

    [Fact]
    public async Task Put_ShouldReturn404_WhenCompanyNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var updateBody = new { Name = "Ažurirana kompanija", Website = "" };
        var response = await client.PutAsJsonAsync("/api/company/99999", updateBody);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenCompanyExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedCompanyAsync(db);

        var response = await client.DeleteAsync("/api/company/100");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenCompanyNotExists()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/company/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

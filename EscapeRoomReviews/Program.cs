using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure EF Core to use MySQL (Pomelo). Connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

builder.Services
    .AddDefaultIdentity<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

builder.Services.AddRazorPages();

builder.Services.AddScoped<EfRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (context.Database.IsRelational())
        await context.Database.MigrateAsync();
    else
        await context.Database.EnsureCreatedAsync();
    await DbSeeder.SeedAsync(context);
    await RoleSeeder.SeedAsync(scope.ServiceProvider);

    var repo = scope.ServiceProvider.GetRequiredService<EfRepository>();
    var escapeRooms = repo.GetAllRooms();
    var trazeniGrad = "Zagreb";
    var trazenaSobaId = 1;

    // LINQ upiti nad ucitanim sobama za demonstraciju osnovnih funkcionalnosti aplikacije.
    // 1) Top 5 najbolje ocijenjenih soba (samo verificirane recenzije)
    var top5NajboljeOcijenjenih = escapeRooms
        .Select(room => new
        {
            room.Id,
            room.Name,
            ProsjecnaOcjena = room.Reviews
                .Where(review => review.IsVerified)
                .Select(review => (double?)review.Rating)
                .Average() ?? 0.0
        })
        .OrderByDescending(item => item.ProsjecnaOcjena)
        .ThenBy(item => item.Name)
        .Take(5)
        .ToList();

    // 2) Sve sobe u odredenom gradu sortirane po tezini
    var sobeUGraduPoTezini = escapeRooms
        .Where(room => room.Location.City.Equals(trazeniGrad, StringComparison.OrdinalIgnoreCase))
        .OrderBy(room => room.Difficulty)
        .ThenBy(room => room.Name)
        .Select(room => new
        {
            room.Id,
            room.Name,
            Grad = room.Location.City,
            Tezina = room.Difficulty
        })
        .ToList();

    // 3) Prosjecna ocjena za odredenu sobu (samo verificirane recenzije)
    var prosjecnaOcjenaSobe = escapeRooms
        .Where(room => room.Id == trazenaSobaId)
        .SelectMany(room => room.Reviews)
        .Where(review => review.IsVerified)
        .Select(review => (double?)review.Rating)
        .Average() ?? 0.0;

    app.Logger.LogInformation("Ucitan primjer podataka: {RoomCount} escape room objekta.", escapeRooms.Count);
    app.Logger.LogInformation("Top 5 verificiranih soba: {Top5}",
        string.Join(", ", top5NajboljeOcijenjenih.Select(item => $"{item.Name} ({item.ProsjecnaOcjena:F2})")));
    app.Logger.LogInformation("Sobe u gradu {Grad} sortirane po tezini: {Sobe}",
        trazeniGrad,
        string.Join(", ", sobeUGraduPoTezini.Select(item => $"{item.Name} [{item.Tezina}]")));
    app.Logger.LogInformation("Prosjecna verificirana ocjena za sobu ID {SobaId}: {Prosjek:F2}", trazenaSobaId, prosjecnaOcjenaSobe);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Trenutno bez autentikacije, ali middleware ostaje spreman za role/policy pravila.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EscapeRoom}/{action=Index}/{id?}");

app.MapRazorPages();

// Pokrece aplikaciju i pocinje slusati HTTP zahtjeve.
app.Run();

public partial class Program { }

using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MockRepository>();

var app = builder.Build();

var repo = app.Services.GetRequiredService<MockRepository>();
var escapeRooms = repo.GetAllRooms();
var trazeniGrad = "Zagreb";
var trazenaSobaId = 1;

// LINQ upiti nad in-memory kolekcijom escapeRooms za demonstraciju osnovnih funkcionalnosti aplikacije.
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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EscapeRoom}/{action=Index}/{id?}");

// Pokrece aplikaciju i pocinje slusati HTTP zahtjeve.
app.Run();

using EscapeRoomReviews.Models.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Primjeri inicijalnih podataka: 3 glavna EscapeRoom objekta s povezanim podacima.
var users = new List<User>
{
    new() { Id = 1, Username = "ana", Email = "ana@example.com", RegisteredAt = DateTime.UtcNow.AddMonths(-10), TotalRoomsPlayed = 14, Role = UserRole.User },
    new() { Id = 2, Username = "marko", Email = "marko@example.com", RegisteredAt = DateTime.UtcNow.AddMonths(-8), TotalRoomsPlayed = 22, Role = UserRole.User },
    new() { Id = 3, Username = "ivana", Email = "ivana@example.com", RegisteredAt = DateTime.UtcNow.AddMonths(-12), TotalRoomsPlayed = 31, Role = UserRole.Moderator }
};

var locations = new List<Location>
{
    new() { Id = 1, City = "Zagreb", Address = "Ilica 101", PostalCode = "10000", Latitude = 45.8144, Longitude = 15.9780 },
    new() { Id = 2, City = "Split", Address = "Poljicka cesta 35", PostalCode = "21000", Latitude = 43.5147, Longitude = 16.4435 },
    new() { Id = 3, City = "Rijeka", Address = "Korzo 20", PostalCode = "51000", Latitude = 45.3271, Longitude = 14.4422 }
};

var companies = new List<Company>
{
    new() { Id = 1, Name = "MindLock", Website = "https://mindlock.hr" },
    new() { Id = 2, Name = "PuzzleGate", Website = "https://puzzlegate.hr" },
    new() { Id = 3, Name = "EscapeLab", Website = "https://escapelab.hr" }
};

var themes = new List<Theme>
{
    new() { Id = 1, Name = "Mystery", IconUrl = "/icons/mystery.svg" },
    new() { Id = 2, Name = "Horror", IconUrl = "/icons/horror.svg" },
    new() { Id = 3, Name = "Sci-Fi", IconUrl = "/icons/scifi.svg" },
    new() { Id = 4, Name = "Adventure", IconUrl = "/icons/adventure.svg" }
};

var escapeRooms = new List<EscapeRoom>
{
    new()
    {
        Id = 1,
        Name = "Tajna Teslinog Laboratorija",
        Description = "Pronađite izgubljene nacrte i pokrenite generator prije nestanka struje.",
        Difficulty = DifficultyLevel.Medium,
        MaxPlayers = 6,
        Price = 95m,
        CreatedAt = DateTime.UtcNow.AddMonths(-6),
        LocationId = locations[0].Id,
        Location = locations[0],
        CompanyId = companies[0].Id,
        Company = companies[0],
        Photos =
        {
            new() { Id = 1, Url = "/images/tesla-1.jpg", EscapeRoomId = 1 },
            new() { Id = 2, Url = "/images/tesla-2.jpg", EscapeRoomId = 1 }
        },
        Reviews =
        {
            new() { Id = 1, Rating = 5, Comment = "Super atmosfera i odlicne zagonetke.", CreatedAt = DateTime.UtcNow.AddDays(-20), IsVerified = true, PlayersCount = 4, UserId = users[0].Id, User = users[0], EscapeRoomId = 1 },
            new() { Id = 2, Rating = 4, Comment = "Logican slijed tragova, preporuka.", CreatedAt = DateTime.UtcNow.AddDays(-12), IsVerified = true, PlayersCount = 5, UserId = users[1].Id, User = users[1], EscapeRoomId = 1 }
        },
        Themes = { themes[0], themes[2] }
    },
    new()
    {
        Id = 2,
        Name = "Ukleta Vila Raven",
        Description = "Istražite napustenu vilu i razotkrijte obiteljsku tajnu prije ponoci.",
        Difficulty = DifficultyLevel.Hard,
        MaxPlayers = 5,
        Price = 110m,
        CreatedAt = DateTime.UtcNow.AddMonths(-4),
        LocationId = locations[1].Id,
        Location = locations[1],
        CompanyId = companies[1].Id,
        Company = companies[1],
        Photos =
        {
            new() { Id = 3, Url = "/images/raven-1.jpg", EscapeRoomId = 2 },
            new() { Id = 4, Url = "/images/raven-2.jpg", EscapeRoomId = 2 }
        },
        Reviews =
        {
            new() { Id = 3, Rating = 5, Comment = "Napeto od prve minute.", CreatedAt = DateTime.UtcNow.AddDays(-15), IsVerified = true, PlayersCount = 5, UserId = users[2].Id, User = users[2], EscapeRoomId = 2 },
            new() { Id = 4, Rating = 4, Comment = "Tezak, ali pravedan room.", CreatedAt = DateTime.UtcNow.AddDays(-7), IsVerified = true, PlayersCount = 4, UserId = users[0].Id, User = users[0], EscapeRoomId = 2 }
        },
        Themes = { themes[1], themes[3] }
    },
    new()
    {
        Id = 3,
        Name = "Misija Mars: Zadnji Signal",
        Description = "Ponovno uspostavite komunikaciju s kolonijom i stabilizirajte reaktor.",
        Difficulty = DifficultyLevel.Expert,
        MaxPlayers = 7,
        Price = 130m,
        CreatedAt = DateTime.UtcNow.AddMonths(-2),
        LocationId = locations[2].Id,
        Location = locations[2],
        CompanyId = companies[2].Id,
        Company = companies[2],
        Photos =
        {
            new() { Id = 5, Url = "/images/mars-1.jpg", EscapeRoomId = 3 },
            new() { Id = 6, Url = "/images/mars-2.jpg", EscapeRoomId = 3 }
        },
        Reviews =
        {
            new() { Id = 5, Rating = 5, Comment = "Najbolja sci-fi soba do sada.", CreatedAt = DateTime.UtcNow.AddDays(-10), IsVerified = true, PlayersCount = 6, UserId = users[1].Id, User = users[1], EscapeRoomId = 3 },
            new() { Id = 6, Rating = 5, Comment = "Odlicna produkcija i tempo.", CreatedAt = DateTime.UtcNow.AddDays(-3), IsVerified = true, PlayersCount = 7, UserId = users[2].Id, User = users[2], EscapeRoomId = 3 }
        },
        Themes = { themes[2], themes[3] }
    }
};

foreach (var room in escapeRooms)
{
    room.Location.EscapeRooms.Add(room);
    room.Company.EscapeRooms.Add(room);

    foreach (var review in room.Reviews)
    {
        review.EscapeRoom = room;
        review.User.Reviews.Add(review);
    }

    foreach (var photo in room.Photos)
    {
        photo.EscapeRoom = room;
    }

    foreach (var theme in room.Themes)
    {
        theme.EscapeRooms.Add(room);
    }
}

var app = builder.Build();

app.Logger.LogInformation("Ucitan primjer podataka: {RoomCount} escape room objekta.", escapeRooms.Count);

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

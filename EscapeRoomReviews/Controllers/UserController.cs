using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers;

public class UserController(ApplicationDbContext db, UserManager<AppUser> userManager, ILogger<UserController> logger) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ILogger<UserController> _logger = logger;

    public async Task<IActionResult> Index()
    {
        var users = await _db.Users
            .Select(u => new
            {
                u.Id,
                u.DisplayName,
                u.FirstName,
                u.LastName,
                u.Email,
                u.RegisteredAt,
                TotalRoomsPlayed = u.Reviews.Count(r => r.DeletedAt == null)
            })
            .ToListAsync();

        var vms = new List<UserIndexViewModel>();
        foreach (var u in users)
        {
            var appUser = await _userManager.FindByIdAsync(u.Id);
            var roles = appUser != null ? await _userManager.GetRolesAsync(appUser) : [];
            vms.Add(new UserIndexViewModel
            {
                Id = u.Id,
                DisplayName = u.DisplayName ?? $"{u.FirstName} {u.LastName}",
                Email = u.Email ?? string.Empty,
                RegisteredAt = u.RegisteredAt,
                TotalRoomsPlayed = u.TotalRoomsPlayed,
                Role = roles.FirstOrDefault() ?? "User"
            });
        }

        return View(vms);
    }

    [Route("korisnik/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var user = await _db.Users
            .Include(u => u.Reviews.Where(r => r.DeletedAt == null))
                .ThenInclude(r => r.EscapeRoom)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            _logger.LogWarning("User {Id} not found", id);
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);

        var vm = new UserDetailsViewModel
        {
            Id = user.Id,
            DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}",
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RegisteredAt = user.RegisteredAt,
            TotalRoomsPlayed = user.Reviews.Count,
            Role = roles.FirstOrDefault() ?? "User",
            Reviews = user.Reviews.ToList()
        };

        return View(vm);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers;

[Authorize(Roles = "Admin")]
public class ManageUserController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ManageUserController> _logger;

    public ManageUserController(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ILogger<ManageUserController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;
    }

    private async Task<List<SelectListItem>> GetRoleOptionsAsync(string? selected = null)
    {
        var roles = new List<string> { "Korisnik" };
        roles.AddRange(await _roleManager.Roles.Select(r => r.Name!).ToListAsync());
        return roles.Select(r => new SelectListItem(r, r, r == selected)).ToList();
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.AsNoTracking().Where(u => u.DeletedAt == null).ToListAsync();
        var viewModels = new List<ManageUsersViewModel>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var role = roles.FirstOrDefault() ?? "Korisnik";
            var reviewCount = _context.Reviews.Count(r => r.AppUserId == u.Id);

            viewModels.Add(new ManageUsersViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email ?? string.Empty,
                Role = role,
                ReviewCount = reviewCount
            });
        }

        return View(viewModels);
    }

    [HttpGet]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User {Id} not found", id);
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Korisnik";

        var model = new ManageUsersViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Role = role
        };

        ViewData["Roles"] = await GetRoleOptionsAsync(role);
        return View(model);
    }

    [HttpPost]
    [ActionName("Edit")]
    public async Task<IActionResult> EditPost(ManageUsersViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Roles"] = await GetRoleOptionsAsync(model.Role);
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            _logger.LogWarning("User {Id} not found", model.Id);
            return NotFound();
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;

        if (user.Email != model.Email)
        {
            await _userManager.SetEmailAsync(user, model.Email);
            await _userManager.SetUserNameAsync(user, model.Email);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (model.Role != "Korisnik")
            await _userManager.AddToRoleAsync(user, model.Role);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            ViewData["Roles"] = await GetRoleOptionsAsync(model.Role);
            return View(model);
        }

        _logger.LogInformation("Updated User {Id}", user.Id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User {Id} not found", id);
            return NotFound();
        }

        user.DeletedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        _logger.LogInformation("Deleted User {Id}", id);
        return RedirectToAction("Index");
    }
}

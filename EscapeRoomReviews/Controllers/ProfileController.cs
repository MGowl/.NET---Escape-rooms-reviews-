using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.Forms;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ApplicationDbContext context,
        ILogger<ProfileController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var logins = await _userManager.GetLoginsAsync(user);
        ViewData["HasExternalLogin"] = logins.Any();

        var model = new ProfileEditViewModel
        {
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ActionName("Edit")]
    public async Task<IActionResult> EditPost(ProfileEditViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var logins = await _userManager.GetLoginsAsync(user);
        var hasExternalLogin = logins.Any();
        ViewData["HasExternalLogin"] = hasExternalLogin;

        if (hasExternalLogin)
            ModelState.Remove(nameof(model.Email));

        if (!ModelState.IsValid)
        {
            model.Email = user.Email ?? string.Empty;
            return View(model);
        }

        user.DisplayName = string.IsNullOrWhiteSpace(model.DisplayName) ? null : model.DisplayName;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;

        if (!hasExternalLogin && user.Email != model.Email)
        {
            await _userManager.SetEmailAsync(user, model.Email);
            await _userManager.SetUserNameAsync(user, model.Email);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            model.Email = user.Email ?? string.Empty;
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("User {Id} updated their profile", user.Id);

        TempData["Success"] = "Profil uspješno ažuriran.";
        return RedirectToAction("Edit");
    }

    [HttpGet]
    [ActionName("MyNewReview")]
    public IActionResult MyNewReviewGet()
    {
        return View(new ReviewCreateModel());
    }

    [HttpPost]
    [ActionName("MyNewReview")]
    public async Task<IActionResult> MyNewReviewPost(ReviewCreateModel model)
    {
        if (model.EscapeRoomId == 0)
            ModelState.AddModelError(nameof(model.EscapeRoomId), "Odaberite escape room.");

        if (!ModelState.IsValid)
        {
            var roomName = await _context.EscapeRooms
                .AsNoTracking()
                .Where(r => r.Id == model.EscapeRoomId && r.DeletedAt == null)
                .Select(r => r.Name)
                .FirstOrDefaultAsync();
            ViewData["EscapeRoomName"] = roomName ?? string.Empty;
            return View(model);
        }

        var review = new Review
        {
            Rating = model.Rating,
            Comment = model.Comment,
            PlayersCount = model.PlayersCount,
            VisitedAt = model.VisitedAt,
            EscapeRoomId = model.EscapeRoomId,
            AppUserId = _userManager.GetUserId(User),
            CreatedAt = DateTime.UtcNow,
            IsVerified = false
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        _logger.LogInformation("User {UserId} created review {ReviewId}", review.AppUserId, review.Id);

        TempData["Success"] = "Recenzija uspješno dodana.";
        return RedirectToAction("MyReviews");
    }

    [HttpGet]
    public async Task<IActionResult> MyReviews()
    {
        var userId = _userManager.GetUserId(User);

        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.AppUserId == userId && r.DeletedAt == null)
            .Include(r => r.EscapeRoom)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new MyReviewViewModel
            {
                Id = r.Id,
                EscapeRoomName = r.EscapeRoom.Name,
                EscapeRoomId = r.EscapeRoomId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return View(reviews);
    }

    [HttpGet]
    [ActionName("EditMyReview")]
    public async Task<IActionResult> EditMyReviewGet(int id)
    {
        var userId = _userManager.GetUserId(User);

        var review = await _context.Reviews
            .AsNoTracking()
            .Include(r => r.EscapeRoom)
            .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

        if (review == null)
        {
            _logger.LogWarning("Review {Id} not found", id);
            return NotFound();
        }

        if (review.AppUserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to edit review {ReviewId} owned by {OwnerId}", userId, id, review.AppUserId);
            return Forbid();
        }

        var model = new ReviewEditModel
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            PlayersCount = review.PlayersCount,
            VisitedAt = review.VisitedAt,
            EscapeRoomId = review.EscapeRoomId,
            IsVerified = review.IsVerified
        };

        ViewData["EscapeRoomName"] = review.EscapeRoom?.Name ?? string.Empty;
        return View(model);
    }

    [HttpPost]
    [ActionName("EditMyReview")]
    public async Task<IActionResult> EditMyReviewPost(ReviewEditModel model)
    {
        var userId = _userManager.GetUserId(User);

        ModelState.Remove(nameof(model.IsVerified));

        if (!ModelState.IsValid)
        {
            var roomName = await _context.EscapeRooms
                .AsNoTracking()
                .Where(r => r.Id == model.EscapeRoomId && r.DeletedAt == null)
                .Select(r => r.Name)
                .FirstOrDefaultAsync();
            ViewData["EscapeRoomName"] = roomName ?? string.Empty;
            return View(model);
        }

        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == model.Id && r.DeletedAt == null);

        if (review == null)
        {
            _logger.LogWarning("Review {Id} not found", model.Id);
            return NotFound();
        }

        if (review.AppUserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to edit review {ReviewId} owned by {OwnerId}", userId, model.Id, review.AppUserId);
            return Forbid();
        }

        review.Rating = model.Rating;
        review.Comment = model.Comment;
        review.PlayersCount = model.PlayersCount;
        review.VisitedAt = model.VisitedAt;
        review.EscapeRoomId = model.EscapeRoomId;
        // IsVerified se ne mijenja ovdje

        await _context.SaveChangesAsync();
        _logger.LogInformation("User {UserId} updated their review {ReviewId}", userId, review.Id);

        TempData["Success"] = "Recenzija uspješno ažurirana.";
        return RedirectToAction("MyReviews");
    }

    [HttpGet]
    public IActionResult SearchEscapeRooms(string term)
    {
        var query = term?.Trim();
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Json(Array.Empty<object>());

        var results = _context.EscapeRooms
            .AsNoTracking()
            .Where(room => room.DeletedAt == null && EF.Functions.Like(room.Name, $"%{query}%"))
            .OrderBy(room => room.Name)
            .Select(room => new { id = room.Id, name = room.Name })
            .Take(10)
            .ToList();

        return Json(results);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMyReview(int id)
    {
        var userId = _userManager.GetUserId(User);

        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

        if (review == null)
        {
            _logger.LogWarning("Review {Id} not found", id);
            return NotFound();
        }

        if (review.AppUserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to delete review {ReviewId} owned by {OwnerId}", userId, id, review.AppUserId);
            return Forbid();
        }

        review.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        _logger.LogInformation("User {UserId} deleted their review {ReviewId}", userId, id);

        return RedirectToAction("MyReviews");
    }
}

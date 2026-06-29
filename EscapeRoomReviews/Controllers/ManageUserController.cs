using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.Forms;

namespace EscapeRoomReviews.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ManageUserController> _logger;

        public ManageUserController(ApplicationDbContext context, ILogger<ManageUserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private void LoadRoleOptions(UserRole? selectedRole = null)
        {
            ViewData["Roles"] = new SelectList(Enum.GetValues(typeof(UserRole)).Cast<UserRole>(), selectedRole);
        }

        public IActionResult Index()
        {
            var users = _context.AppUsers
                .AsNoTracking()
                .Where(user => user.DeletedAt == null)
                .ToList();

            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadRoleOptions();
            return View(new UserCreateModel());
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult CreatePost(UserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadRoleOptions(model.Role);
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                TotalRoomsPlayed = model.TotalRoomsPlayed,
                Role = model.Role,
                RegisteredAt = DateTime.UtcNow
            };

            _context.AppUsers.Add(user);
            _context.SaveChanges();
            _logger.LogInformation("Created User {Username}", user.Username);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var user = _context.AppUsers
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == id && u.DeletedAt == null);

            if (user == null)
            {
                _logger.LogWarning("User {Id} not found", id);
                return NotFound();
            }

            var model = new UserEditModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                TotalRoomsPlayed = user.TotalRoomsPlayed,
                Role = user.Role
            };

            LoadRoleOptions(model.Role);
            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(UserEditModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadRoleOptions(model.Role);
                return View(model);
            }

            var user = _context.AppUsers.FirstOrDefault(u => u.Id == model.Id && u.DeletedAt == null);
            if (user == null)
            {
                _logger.LogWarning("User {Id} not found", model.Id);
                return NotFound();
            }

            user.Username = model.Username;
            user.Email = model.Email;
            user.TotalRoomsPlayed = model.TotalRoomsPlayed;
            user.Role = model.Role;

            _context.SaveChanges();
            _logger.LogInformation("Updated User {Id}", user.Id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.AppUsers.FirstOrDefault(u => u.Id == id && u.DeletedAt == null);
            if (user == null)
            {
                _logger.LogWarning("User {Id} not found", id);
                return NotFound();
            }

            user.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
            _logger.LogInformation("Deleted User {Id}", id);

            return RedirectToAction("Index");
        }
    }
}

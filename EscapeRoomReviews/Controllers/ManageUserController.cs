using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.Forms;

namespace EscapeRoomReviews.Controllers
{
    public class ManageUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private void LoadRoleOptions(UserRole? selectedRole = null)
        {
            ViewData["Roles"] = new SelectList(Enum.GetValues(typeof(UserRole)).Cast<UserRole>(), selectedRole);
        }

        public IActionResult Index()
        {
            var users = _context.Users
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

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == id && u.DeletedAt == null);

            if (user == null) return NotFound();

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

            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id && u.DeletedAt == null);
            if (user == null) return NotFound();

            user.Username = model.Username;
            user.Email = model.Email;
            user.TotalRoomsPlayed = model.TotalRoomsPlayed;
            user.Role = model.Role;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.DeletedAt == null);
            if (user == null) return NotFound();

            user.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

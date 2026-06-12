using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.Forms;

namespace EscapeRoomReviews.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class ManageThemeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageThemeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var themes = _context.Themes
                .AsNoTracking()
                .Where(theme => theme.DeletedAt == null)
                .Include(theme => theme.EscapeRooms)
                .ToList();

            return View(themes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ThemeCreateModel());
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult CreatePost(ThemeCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var theme = new Theme
            {
                Name = model.Name,
                IconUrl = model.IconUrl
            };

            _context.Themes.Add(theme);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var theme = _context.Themes
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id && t.DeletedAt == null);

            if (theme == null) return NotFound();

            var model = new ThemeEditModel
            {
                Id = theme.Id,
                Name = theme.Name,
                IconUrl = theme.IconUrl
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(ThemeEditModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var theme = _context.Themes.FirstOrDefault(t => t.Id == model.Id && t.DeletedAt == null);
            if (theme == null) return NotFound();

            theme.Name = model.Name;
            theme.IconUrl = model.IconUrl;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var theme = _context.Themes.FirstOrDefault(t => t.Id == id && t.DeletedAt == null);
            if (theme == null) return NotFound();

            theme.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

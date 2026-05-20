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
    public class ManageEscapeRoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageEscapeRoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetLocationName(int? id)
        {
            if (!id.HasValue || id.Value == 0) return null;

            return _context.Locations
                .AsNoTracking()
                .Where(location => location.Id == id.Value)
                .Select(location => location.City + " - " + location.Address)
                .FirstOrDefault();
        }

            private string? GetCompanyName(int? id)
            {
                if (!id.HasValue || id.Value == 0) return null;

                return _context.Companies
                .AsNoTracking()
                .Where(company => company.Id == id.Value)
                .Select(company => company.Name)
                .FirstOrDefault();
            }

        private void LoadLocationOptions(int? selectedId = null)
        {
            var locations = _context.Locations
                .AsNoTracking()
                .OrderBy(location => location.City)
                .ThenBy(location => location.Address)
                .Select(location => new
                {
                    location.Id,
                    Name = location.City + " - " + location.Address
                })
                .ToList();

            ViewData["Locations"] = new SelectList(locations, "Id", "Name", selectedId);
        }


        private void LoadThemeOptions(IEnumerable<int>? selectedIds = null)
        {
            var selectedSet = selectedIds?.ToHashSet() ?? new HashSet<int>();
            var themes = _context.Themes
                .AsNoTracking()
                .Where(theme => theme.DeletedAt == null)
                .OrderBy(theme => theme.Name)
                .Select(theme => new SelectListItem
                {
                    Value = theme.Id.ToString(),
                    Text = theme.Name,
                    Selected = selectedSet.Contains(theme.Id)
                })
                .ToList();

            ViewData["Themes"] = themes;
        }

        public IActionResult Index()
        {
            var rooms = _context.EscapeRooms
                .AsNoTracking()
                .Where(room => room.DeletedAt == null)
                .Include(room => room.Location)
                .Include(room => room.Company)
                .ToList();

            return View(rooms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadThemeOptions();
            return View(new EscapeRoomCreateModel());
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult CreatePost(EscapeRoomCreateModel model)
        {
            if (model.LocationId == 0)
            {
                ModelState.AddModelError(nameof(model.LocationId), "Odaberite lokaciju.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["LocationName"] = GetLocationName(model.LocationId);
                ViewData["CompanyName"] = GetCompanyName(model.CompanyId);
                LoadThemeOptions(model.ThemeIds);
                return View(model);
            }

            var selectedThemes = _context.Themes
                .Where(theme => model.ThemeIds.Contains(theme.Id) && theme.DeletedAt == null)
                .ToList();

            var room = new EscapeRoom
            {
                Name = model.Name,
                Description = model.Description,
                Difficulty = model.Difficulty,
                MaxPlayers = model.MaxPlayers,
                Price = model.Price,
                LocationId = model.LocationId,
                CompanyId = model.CompanyId,
                Themes = selectedThemes,
                CreatedAt = DateTime.UtcNow
            };

            _context.EscapeRooms.Add(room);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var room = _context.EscapeRooms
                .AsNoTracking()
                .Include(r => r.Themes)
                .FirstOrDefault(r => r.Id == id && r.DeletedAt == null);

            if (room == null) return NotFound();

            var model = new EscapeRoomEditModel
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                Difficulty = room.Difficulty,
                MaxPlayers = room.MaxPlayers,
                Price = room.Price,
                LocationId = room.LocationId,
                CompanyId = room.CompanyId,
                ThemeIds = room.Themes.Select(theme => theme.Id).ToList()
            };
            ViewData["LocationName"] = GetLocationName(model.LocationId);
            ViewData["CompanyName"] = GetCompanyName(model.CompanyId);
            LoadThemeOptions(model.ThemeIds);
            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(EscapeRoomEditModel model)
        {
            if (model.LocationId == 0)
            {
                ModelState.AddModelError(nameof(model.LocationId), "Odaberite lokaciju.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["LocationName"] = GetLocationName(model.LocationId);
                ViewData["CompanyName"] = GetCompanyName(model.CompanyId);
                LoadThemeOptions(model.ThemeIds);
                return View(model);
            }

            var room = _context.EscapeRooms
                .Include(r => r.Themes)
                .FirstOrDefault(r => r.Id == model.Id && r.DeletedAt == null);
            if (room == null) return NotFound();

            room.Name = model.Name;
            room.Description = model.Description;
            room.Difficulty = model.Difficulty;
            room.MaxPlayers = model.MaxPlayers;
            room.Price = model.Price;
            room.LocationId = model.LocationId;
            room.CompanyId = model.CompanyId;

            room.Themes.Clear();
            if (model.ThemeIds.Count > 0)
            {
                var selectedThemes = _context.Themes
                    .Where(theme => model.ThemeIds.Contains(theme.Id) && theme.DeletedAt == null)
                    .ToList();

                foreach (var theme in selectedThemes)
                {
                    room.Themes.Add(theme);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var room = _context.EscapeRooms.FirstOrDefault(r => r.Id == id && r.DeletedAt == null);
            if (room == null) return NotFound();

            room.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SearchLocations(string term)
        {
            var query = term?.Trim();
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            var results = _context.Locations
                .AsNoTracking()
                .Where(location =>
                    EF.Functions.Like(location.City, $"%{query}%") ||
                    EF.Functions.Like(location.Address, $"%{query}%"))
                .OrderBy(location => location.City)
                .ThenBy(location => location.Address)
                .Select(location => new
                {
                    id = location.Id,
                    name = location.City + " - " + location.Address
                })
                .Take(10)
                .ToList();

            return Json(results);
        }

        [HttpGet]
        public IActionResult SearchCompanies(string term)
        {
            var query = term?.Trim();
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            var results = _context.Companies
                .AsNoTracking()
                .Where(company => EF.Functions.Like(company.Name, $"%{query}%"))
                .OrderBy(company => company.Name)
                .Select(company => new { id = company.Id, name = company.Name })
                .Take(10)
                .ToList();

            return Json(results);
        }
    }
}

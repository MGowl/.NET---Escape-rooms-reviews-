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

        private void LoadCompanyOptions(int? selectedId = null)
        {
            var companies = _context.Companies
                .AsNoTracking()
                .OrderBy(company => company.Name)
                .Select(company => new { company.Id, company.Name })
                .ToList();

            ViewData["Companies"] = new SelectList(companies, "Id", "Name", selectedId);
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
            LoadLocationOptions();
            LoadCompanyOptions();
            return View(new EscapeRoomCreateModel());
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult CreatePost(EscapeRoomCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadLocationOptions(model.LocationId);
                LoadCompanyOptions(model.CompanyId);
                return View(model);
            }

            var room = new EscapeRoom
            {
                Name = model.Name,
                Description = model.Description,
                Difficulty = model.Difficulty,
                MaxPlayers = model.MaxPlayers,
                Price = model.Price,
                LocationId = model.LocationId,
                CompanyId = model.CompanyId,
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
                CompanyId = room.CompanyId
            };
            LoadLocationOptions(model.LocationId);
            LoadCompanyOptions(model.CompanyId);
            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(EscapeRoomEditModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadLocationOptions(model.LocationId);
                LoadCompanyOptions(model.CompanyId);
                return View(model);
            }

            var room = _context.EscapeRooms.FirstOrDefault(r => r.Id == model.Id && r.DeletedAt == null);
            if (room == null) return NotFound();

            room.Name = model.Name;
            room.Description = model.Description;
            room.Difficulty = model.Difficulty;
            room.MaxPlayers = model.MaxPlayers;
            room.Price = model.Price;
            room.LocationId = model.LocationId;
            room.CompanyId = model.CompanyId;

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
    }
}

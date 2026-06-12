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
    public class ManageLocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageLocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var locations = _context.Locations
                .AsNoTracking()
                .Where(location => location.DeletedAt == null)
                .Include(location => location.EscapeRooms)
                .ToList();

            return View(locations);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new LocationCreateModel());
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult CreatePost(LocationCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var location = new Location
            {
                City = model.City,
                Address = model.Address,
                PostalCode = model.PostalCode,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };

            _context.Locations.Add(location);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var location = _context.Locations
                .AsNoTracking()
                .FirstOrDefault(l => l.Id == id && l.DeletedAt == null);

            if (location == null) return NotFound();

            var model = new LocationEditModel
            {
                Id = location.Id,
                City = location.City,
                Address = location.Address,
                PostalCode = location.PostalCode,
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(LocationEditModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var location = _context.Locations.FirstOrDefault(l => l.Id == model.Id && l.DeletedAt == null);
            if (location == null) return NotFound();

            location.City = model.City;
            location.Address = model.Address;
            location.PostalCode = model.PostalCode;
            location.Latitude = model.Latitude;
            location.Longitude = model.Longitude;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var location = _context.Locations.FirstOrDefault(l => l.Id == id && l.DeletedAt == null);
            if (location == null) return NotFound();

            location.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

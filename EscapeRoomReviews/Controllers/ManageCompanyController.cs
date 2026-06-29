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
    public class ManageCompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ManageCompanyController> _logger;

        public ManageCompanyController(ApplicationDbContext context, ILogger<ManageCompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var companies = _context.Companies
                .AsNoTracking()
                .Where(company => company.DeletedAt == null)
                .Include(company => company.EscapeRooms)
                .ToList();

            return View(companies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CompanyCreateModel());
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult CreatePost(CompanyCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var company = new Company
            {
                Name = model.Name,
                Website = model.Website
            };

            _context.Companies.Add(company);
            _context.SaveChanges();
            _logger.LogInformation("Created Company {Name}", company.Name);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var company = _context.Companies
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id && c.DeletedAt == null);

            if (company == null)
            {
                _logger.LogWarning("Company {Id} not found", id);
                return NotFound();
            }

            var model = new CompanyEditModel
            {
                Id = company.Id,
                Name = company.Name,
                Website = company.Website
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(CompanyEditModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var company = _context.Companies.FirstOrDefault(c => c.Id == model.Id && c.DeletedAt == null);
            if (company == null)
            {
                _logger.LogWarning("Company {Id} not found", model.Id);
                return NotFound();
            }

            company.Name = model.Name;
            company.Website = model.Website;

            _context.SaveChanges();
            _logger.LogInformation("Updated Company {Id}", company.Id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var company = _context.Companies.FirstOrDefault(c => c.Id == id && c.DeletedAt == null);
            if (company == null)
            {
                _logger.LogWarning("Company {Id} not found", id);
                return NotFound();
            }

            company.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
            _logger.LogInformation("Deleted Company {Id}", id);

            return RedirectToAction("Index");
        }
    }
}

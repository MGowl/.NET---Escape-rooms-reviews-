using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class CompanyController : Controller
    {
        private readonly EfRepository _repo;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(EfRepository repo, ILogger<CompanyController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var companies = _repo.GetAllCompanies()
                .Select(company => new CompanyIndexViewModel
                {
                    Id = company.Id,
                    Name = company.Name,
                    Website = company.Website,
                    RoomCount = company.EscapeRooms.Count
                })
                .ToList();

            return View(companies);
        }

        [Route("tvrtka/{id}")]
        public IActionResult Details(int id)
        {
            var company = _repo.GetAllCompanies().FirstOrDefault(c => c.Id == id);
            if (company == null)
            {
                _logger.LogWarning("Company {Id} not found", id);
                return NotFound();
            }
            return View(company);
        }
    }
}

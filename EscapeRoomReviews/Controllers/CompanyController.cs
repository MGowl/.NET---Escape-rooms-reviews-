using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class CompanyController : Controller
    {
        private readonly EfRepository _repo;

        public CompanyController(EfRepository repo)
        {
            _repo = repo;
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
            if (company == null) return NotFound();
            return View(company);
        }
    }
}

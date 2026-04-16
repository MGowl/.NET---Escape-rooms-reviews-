using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class CompanyController : Controller
    {
        private readonly MockRepository _repo;

        public CompanyController(MockRepository repo)
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
    }
}

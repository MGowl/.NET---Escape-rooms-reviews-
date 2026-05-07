using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class LocationController : Controller
    {
        private readonly EfRepository _repo;

        public LocationController(EfRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var locations = _repo.GetAllLocations()
                .Select(location => new LocationIndexViewModel
                {
                    Id = location.Id,
                    City = location.City,
                    Address = location.Address,
                    PostalCode = location.PostalCode,
                    RoomCount = location.EscapeRooms.Count
                })
                .ToList();

            return View(locations);
        }

        [Route("lokacija/{id}")]
        public IActionResult Details(int id)
        {
            var location = _repo.GetAllLocations().FirstOrDefault(l => l.Id == id);
            if (location == null) return NotFound();
            return View(location);
        }
    }
}

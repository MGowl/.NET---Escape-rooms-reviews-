using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class LocationController : Controller
    {
        private readonly MockRepository _repo;

        public LocationController(MockRepository repo)
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
    }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class EscapeRoomController : Controller
    {
        private readonly MockRepository _repo;

        public EscapeRoomController(MockRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var rooms = _repo.GetAllRooms()
                .Select(room => new EscapeRoomIndexViewModel
                {
                    Id = room.Id,
                    Name = room.Name,
                    Difficulty = room.Difficulty.ToString(),
                    Price = room.Price,
                    City = room.Location.City,
                    CompanyName = room.Company.Name,
                    AverageRating = room.Reviews.Count == 0
                        ? 0.0
                        : room.Reviews.Average(review => review.Rating),
                    ReviewCount = room.Reviews.Count,
                    Themes = room.Themes.Select(theme => theme.Name).ToList()
                })
                .ToList();

            return View(rooms);
        }
    }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class EscapeRoomController : Controller
    {
        private readonly EfRepository _repo;
        private readonly ILogger<EscapeRoomController> _logger;

        public EscapeRoomController(EfRepository repo, ILogger<EscapeRoomController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public IActionResult Index(string[]? city, string[]? difficulty, string[]? theme, string? sort)
        {
            var roomQuery = _repo.GetAllRooms().AsEnumerable();

            if (city != null && city.Length > 0)
            {
                roomQuery = roomQuery.Where(room => city.Contains(room.Location.City));
            }

            if (difficulty != null && difficulty.Length > 0)
            {
                roomQuery = roomQuery.Where(room => difficulty.Contains(room.Difficulty.ToString()));
            }

            if (theme != null && theme.Length > 0)
            {
                roomQuery = roomQuery.Where(room => room.Themes.Any(t => theme.Contains(t.Name)));
            }

            var sortKey = string.IsNullOrWhiteSpace(sort) ? "rating" : sort;
            roomQuery = sortKey switch
            {
                "price-asc" => roomQuery.OrderBy(room => room.Price),
                "price-desc" => roomQuery.OrderByDescending(room => room.Price),
                _ => roomQuery.OrderByDescending(room => room.Reviews.Count == 0
                    ? 0.0
                    : room.Reviews.Average(review => review.Rating))
            };

            var rooms = roomQuery
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
                    Themes = room.Themes.Select(theme => new EscapeRoomReviews.ViewModels.ThemeSlimViewModel { Name = theme.Name, IconUrl = theme.IconUrl }).ToList()
                })
                .ToList();

            ViewData["SelectedCities"] = city?.ToList() ?? new List<string>();
            ViewData["SelectedDifficulties"] = difficulty?.ToList() ?? new List<string>();
            ViewData["SelectedThemes"] = theme?.ToList() ?? new List<string>();
            ViewData["SelectedSort"] = sortKey;

            return View(rooms);
        }

        [Route("soba/{id}")]
        public IActionResult Details(int id)
        {
            var room = _repo.GetRoomById(id);
            if (room == null)
            {
                _logger.LogWarning("EscapeRoom {Id} not found", id);
                return NotFound();
            }
            return View(room);
        }
    }
}

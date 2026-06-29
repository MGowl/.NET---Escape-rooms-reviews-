using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class ThemeController : Controller
    {
        private readonly EfRepository _repo;
        private readonly ILogger<ThemeController> _logger;

        public ThemeController(EfRepository repo, ILogger<ThemeController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var themes = _repo.GetAllThemes()
                .Select(theme => new ThemeIndexViewModel
                {
                    Id = theme.Id,
                    Name = theme.Name,
                    RoomCount = theme.EscapeRooms.Count,
                    IconUrl = theme.IconUrl
                })
                .ToList();

            return View(themes);
        }

        [Route("teme/{id}")]
        public IActionResult Details(int id)
        {
            var theme = _repo.GetAllThemes().FirstOrDefault(t => t.Id == id);
            if (theme == null)
            {
                _logger.LogWarning("Theme {Id} not found", id);
                return NotFound();
            }
            return View(theme);
        }
    }
}

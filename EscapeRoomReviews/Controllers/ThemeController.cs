using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class ThemeController : Controller
    {
        private readonly MockRepository _repo;

        public ThemeController(MockRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var themes = _repo.GetAllThemes()
                .Select(theme => new ThemeIndexViewModel
                {
                    Id = theme.Id,
                    Name = theme.Name,
                    RoomCount = theme.EscapeRooms.Count
                })
                .ToList();

            return View(themes);
        }

        public IActionResult Details(int id)
        {
            var theme = _repo.GetAllThemes().FirstOrDefault(t => t.Id == id);
            if (theme == null) return NotFound();
            return View(theme);
        }
    }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class UserController : Controller
    {
        private readonly EfRepository _repo;
        private readonly ILogger<UserController> _logger;

        public UserController(EfRepository repo, ILogger<UserController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var users = _repo.GetAllUsers()
                .Select(user => new UserIndexViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    TotalRoomsPlayed = user.TotalRoomsPlayed,
                    RegisteredAt = user.RegisteredAt
                })
                .ToList();

            return View(users);
        }

        [Route("korisnik/{id}")]
        public IActionResult Details(int id)
        {
            var user = _repo.GetAllUsers().FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("User {Id} not found", id);
                return NotFound();
            }
            return View(user);
        }
    }
}

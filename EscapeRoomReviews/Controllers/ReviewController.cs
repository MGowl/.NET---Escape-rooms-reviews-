using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EscapeRoomReviews.Repositories;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class ReviewController : Controller
    {
        private readonly MockRepository _repo;

        public ReviewController(MockRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var reviews = _repo.GetAllReviews()
                .Select(review => new ReviewIndexViewModel
                {
                    Id = review.Id,
                    RoomName = review.EscapeRoom.Name,
                    Username = review.User.Username,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    IsVerified = review.IsVerified
                })
                .ToList();

            return View(reviews);
        }
    }
}

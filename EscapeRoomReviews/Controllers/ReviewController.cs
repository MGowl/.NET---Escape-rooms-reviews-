using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.ViewModels;

namespace EscapeRoomReviews.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var reviews = _context.Reviews
                .AsNoTracking()
                .Where(review => review.DeletedAt == null)
                .Include(review => review.EscapeRoom)
                .Include(review => review.User)
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

        [Route("recenzija/{id}")]
        public IActionResult Details(int id)
        {
            var review = _context.Reviews
                .AsNoTracking()
                .Include(r => r.EscapeRoom)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id && r.DeletedAt == null);
            if (review == null) return NotFound();
            return View(review);
        }

    }
}

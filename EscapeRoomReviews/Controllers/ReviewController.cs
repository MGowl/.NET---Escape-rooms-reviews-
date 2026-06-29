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
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(ApplicationDbContext context, ILogger<ReviewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var reviews = _context.Reviews
                .AsNoTracking()
                .Where(review => review.DeletedAt == null)
                .Include(review => review.EscapeRoom)
                .Include(review => review.AppUser)
                .OrderByDescending(review => review.CreatedAt)
                .Select(review => new ReviewIndexViewModel
                {
                    Id = review.Id,
                    RoomName = review.EscapeRoom.Name,
                    DisplayName = review.AppUser != null
                        ? (review.AppUser.DisplayName ?? (review.AppUser.FirstName + " " + review.AppUser.LastName))
                        : string.Empty,
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
                    .ThenInclude(er => er.Location)
                .Include(r => r.AppUser)
                .FirstOrDefault(r => r.Id == id && r.DeletedAt == null);
            if (review == null)
            {
                _logger.LogWarning("Review {Id} not found", id);
                return NotFound();
            }
            return View(review);
        }

    }
}

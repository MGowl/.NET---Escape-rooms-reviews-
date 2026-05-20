using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.Forms;
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

        private string? GetEscapeRoomName(int? id)
        {
            if (!id.HasValue || id.Value == 0) return null;

            return _context.EscapeRooms
                .AsNoTracking()
                .Where(room => room.DeletedAt == null && room.Id == id.Value)
                .Select(room => room.Name)
                .FirstOrDefault();
        }

        private void LoadEscapeRoomOptions(int? selectedId = null)
        {
            var rooms = _context.EscapeRooms
                .AsNoTracking()
                .Where(room => room.DeletedAt == null)
                .OrderBy(room => room.Name)
                .Select(room => new { room.Id, room.Name })
                .ToList();

            ViewData["EscapeRooms"] = new SelectList(rooms, "Id", "Name", selectedId);
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

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ReviewCreateModel());
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult CreatePost(ReviewCreateModel model)
        {
            if (model.EscapeRoomId == 0)
            {
                ModelState.AddModelError(nameof(model.EscapeRoomId), "Odaberite escape room.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["EscapeRoomName"] = GetEscapeRoomName(model.EscapeRoomId);
                return View(model);
            }

            var userId = _context.Users
                .AsNoTracking()
                .Select(user => user.Id)
                .FirstOrDefault();

            if (userId == 0)
            {
                ModelState.AddModelError(string.Empty, "No users available to assign the review.");
                ViewData["EscapeRoomName"] = GetEscapeRoomName(model.EscapeRoomId);
                return View(model);
            }

            var review = new Review
            {
                Rating = model.Rating,
                Comment = model.Comment,
                PlayersCount = model.PlayersCount,
                EscapeRoomId = model.EscapeRoomId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsVerified = false
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var review = _context.Reviews
                .AsNoTracking()
                .FirstOrDefault(r => r.Id == id && r.DeletedAt == null);

            if (review == null) return NotFound();

            var model = new ReviewEditModel
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                PlayersCount = review.PlayersCount,
                EscapeRoomId = review.EscapeRoomId,
                IsVerified = review.IsVerified
            };
            ViewData["EscapeRoomName"] = GetEscapeRoomName(model.EscapeRoomId);
            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(ReviewEditModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["EscapeRoomName"] = GetEscapeRoomName(model.EscapeRoomId);
                return View(model);
            }

            var review = _context.Reviews.FirstOrDefault(r => r.Id == model.Id && r.DeletedAt == null);
            if (review == null) return NotFound();

            review.Rating = model.Rating;
            review.Comment = model.Comment;
            review.PlayersCount = model.PlayersCount;
            review.EscapeRoomId = model.EscapeRoomId;
            review.IsVerified = model.IsVerified;

            _context.SaveChanges();

            return RedirectToAction("Index");
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

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id && r.DeletedAt == null);
            if (review == null) return NotFound();

            review.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SearchEscapeRooms(string term)
        {
            var query = term?.Trim();
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            var results = _context.EscapeRooms
                .AsNoTracking()
                .Where(room => room.DeletedAt == null && EF.Functions.Like(room.Name, $"%{query}%"))
                .OrderBy(room => room.Name)
                .Select(room => new { id = room.Id, name = room.Name })
                .Take(10)
                .ToList();

            return Json(results);
        }
    }
}

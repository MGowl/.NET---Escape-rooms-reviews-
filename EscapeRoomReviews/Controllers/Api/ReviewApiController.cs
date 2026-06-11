using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;

namespace EscapeRoomReviews.Controllers.Api;

[ApiController]
[Route("api/review")]
public class ReviewApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReviewApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Mapira Review entitet u ReviewDTO.
    /// </summary>
    private static ReviewDTO ToDTO(Review review)
    {
        return new ReviewDTO
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            PlayersCount = review.PlayersCount,
            VisitedAt = review.VisitedAt,
            IsVerified = review.IsVerified,
            CreatedAt = review.CreatedAt,
            EscapeRoomName = review.EscapeRoom.Name,
            Username = review.User.Username
        };
    }

    /// <summary>
    /// GET /api/review - Vraća sve aktivne recenzije (gdje je DeletedAt == null).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetAll()
    {
        var reviews = await _context.Reviews
            .Where(r => r.DeletedAt == null)
            .Include(r => r.EscapeRoom)
            .Include(r => r.User)
            .ToListAsync();

        return Ok(reviews.Select(ToDTO).ToList());
    }

    /// <summary>
    /// GET /api/review/{id} - Vraća recenziju po ID-u.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDTO>> GetById(int id)
    {
        var review = await _context.Reviews
            .Where(r => r.Id == id && r.DeletedAt == null)
            .Include(r => r.EscapeRoom)
            .Include(r => r.User)
            .FirstOrDefaultAsync();

        if (review == null)
        {
            return NotFound();
        }

        return Ok(ToDTO(review));
    }

    /// <summary>
    /// GET /api/review/search/{q} - Pretraga recenzija po komentaru.
    /// </summary>
    [HttpGet("search/{q}")]
    public async Task<ActionResult<IEnumerable<ReviewDTO>>> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Pojam za pretragu je obavezan." });
        }

        var reviews = await _context.Reviews
            .Where(r => r.DeletedAt == null && r.Comment.Contains(q))
            .Include(r => r.EscapeRoom)
            .Include(r => r.User)
            .ToListAsync();

        return Ok(reviews.Select(ToDTO).ToList());
    }

    /// <summary>
    /// POST /api/review - Kreira novu recenziju.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReviewDTO>> Create([FromBody] ReviewUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (dto.Rating < 1 || dto.Rating > 5)
        {
            return BadRequest(new { message = "Ocjena mora biti između 1 i 5." });
        }

        var escapeRoomExists = await _context.EscapeRooms
            .AnyAsync(er => er.Id == dto.EscapeRoomId && er.DeletedAt == null);
        if (!escapeRoomExists)
        {
            return BadRequest(new { message = "Escape room nije pronađen." });
        }

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == dto.UserId && u.DeletedAt == null);
        if (!userExists)
        {
            return BadRequest(new { message = "Korisnik nije pronađen." });
        }

        var review = new Review
        {
            Rating = dto.Rating,
            Comment = dto.Comment,
            PlayersCount = dto.PlayersCount,
            VisitedAt = dto.VisitedAt,
            EscapeRoomId = dto.EscapeRoomId,
            UserId = dto.UserId,
            IsVerified = dto.IsVerified,
            CreatedAt = DateTime.UtcNow,
            DeletedAt = null
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var createdReview = await _context.Reviews
            .Where(r => r.Id == review.Id)
            .Include(r => r.EscapeRoom)
            .Include(r => r.User)
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = review.Id }, ToDTO(createdReview));
    }

    /// <summary>
    /// PUT /api/review/{id} - Ažurira recenziju po ID-u.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewDTO>> Update(int id, [FromBody] ReviewUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var review = await _context.Reviews
            .Where(r => r.Id == id && r.DeletedAt == null)
            .Include(r => r.EscapeRoom)
            .Include(r => r.User)
            .FirstOrDefaultAsync();

        if (review == null)
        {
            return NotFound();
        }

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        review.PlayersCount = dto.PlayersCount;
        review.VisitedAt = dto.VisitedAt;
        review.EscapeRoomId = dto.EscapeRoomId;
        review.IsVerified = dto.IsVerified;

        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();

        var updatedReview = await _context.Reviews
            .Where(r => r.Id == id)
            .Include(r => r.EscapeRoom)
            .Include(r => r.User)
            .FirstAsync();

        return Ok(ToDTO(updatedReview));
    }

    /// <summary>
    /// DELETE /api/review/{id} - Soft delete recenzije.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _context.Reviews
            .Where(r => r.Id == id && r.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (review == null)
        {
            return NotFound();
        }

        review.DeletedAt = DateTime.UtcNow;
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;

namespace EscapeRoomReviews.Controllers.Api;

[ApiController]
[Route("api/user")]
public class UserApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Mapira User entitet u UserDTO.
    /// </summary>
    private static UserDTO ToDTO(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            TotalRoomsPlayed = user.TotalRoomsPlayed,
            Role = user.Role.ToString(),
            RegisteredAt = user.RegisteredAt
        };
    }

    /// <summary>
    /// GET /api/user - Vraća sve aktivne korisnike (gdje je DeletedAt == null).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
    {
        var users = await _context.Users
            .Where(u => u.DeletedAt == null)
            .ToListAsync();

        return Ok(users.Select(ToDTO).ToList());
    }

    /// <summary>
    /// GET /api/user/{id} - Vraća korisnika po ID-u.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetById(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return Ok(ToDTO(user));
    }

    /// <summary>
    /// GET /api/user/search/{q} - Pretraga korisnika po korisničkom imenu i emailu.
    /// </summary>
    [HttpGet("search/{q}")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Pojam za pretragu je obavezan." });
        }

        var users = await _context.Users
            .Where(u => u.DeletedAt == null &&
                        (u.Username.Contains(q) || u.Email.Contains(q)))
            .ToListAsync();

        return Ok(users.Select(ToDTO).ToList());
    }

    /// <summary>
    /// POST /api/user - Kreira novog korisnika.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDTO>> Create([FromBody] UserUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            TotalRoomsPlayed = dto.TotalRoomsPlayed,
            Role = dto.Role,
            RegisteredAt = DateTime.UtcNow,
            DeletedAt = null
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, ToDTO(user));
    }

    /// <summary>
    /// PUT /api/user/{id} - Ažurira korisnika po ID-u.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDTO>> Update(int id, [FromBody] UserUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users
            .Where(u => u.Id == id && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.TotalRoomsPlayed = dto.TotalRoomsPlayed;
        user.Role = dto.Role;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(ToDTO(user));
    }

    /// <summary>
    /// DELETE /api/user/{id} - Soft delete korisnika.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        user.DeletedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
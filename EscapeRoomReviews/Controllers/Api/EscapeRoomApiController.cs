using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;

namespace EscapeRoomReviews.Controllers.Api;

[ApiController]
[Route("api/escaperoom")]
public class EscapeRoomApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EscapeRoomApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Mapira EscapeRoom entitet u EscapeRoomDTO
    /// </summary>
    private EscapeRoomDTO ToDTO(EscapeRoom room)
    {
        return new EscapeRoomDTO
        {
            Id = room.Id,
            Name = room.Name,
            Description = room.Description,
            Difficulty = (int)room.Difficulty,
            MaxPlayers = room.MaxPlayers,
            Price = room.Price,
            Location = new LocationDTO
            {
                Id = room.Location.Id,
                City = room.Location.City,
                Address = room.Location.Address
            },
            Company = new CompanyDTO
            {
                Id = room.Company.Id,
                Name = room.Company.Name
            },
            Themes = room.Themes
                .Select(t => new ThemeDTO { Id = t.Id, Name = t.Name })
                .ToList()
        };
    }

    /// <summary>
    /// GET /api/escaperoom - Vraća sve aktivne escape roome (gdje je DeletedAt == null)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EscapeRoomDTO>>> GetAll()
    {
        var rooms = await _context.EscapeRooms
            .Where(r => r.DeletedAt == null)
            .Include(r => r.Location)
            .Include(r => r.Company)
            .Include(r => r.Themes)
            .ToListAsync();

        var dtos = rooms.Select(ToDTO).ToList();
        return Ok(dtos);
    }

    /// <summary>
    /// GET /api/escaperoom/{id} - Vraća escape room po Id-u
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EscapeRoomDTO>> GetById(int id)
    {
        var room = await _context.EscapeRooms
            .Where(r => r.Id == id && r.DeletedAt == null)
            .Include(r => r.Location)
            .Include(r => r.Company)
            .Include(r => r.Themes)
            .FirstOrDefaultAsync();

        if (room == null)
        {
            return NotFound(new { message = $"Escape room sa ID {id} nije pronađen." });
        }

        return Ok(ToDTO(room));
    }

    /// <summary>
    /// GET /api/escaperoom/search/{q} - Pretraga po naziv i opis
    /// </summary>
    [HttpGet("search/{q}")]
    public async Task<ActionResult<IEnumerable<EscapeRoomDTO>>> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Pretraživani pojam ne može biti prazan." });
        }

        var query = q.ToLower();
        var rooms = await _context.EscapeRooms
            .Where(r => r.DeletedAt == null &&
                        (r.Name.ToLower().Contains(query) || r.Description.ToLower().Contains(query)))
            .Include(r => r.Location)
            .Include(r => r.Company)
            .Include(r => r.Themes)
            .ToListAsync();

        var dtos = rooms.Select(ToDTO).ToList();
        return Ok(dtos);
    }

    /// <summary>
    /// POST /api/escaperoom - Kreira novi escape room
    /// </summary>
    [Authorize(Roles = "Admin,Editor")]
    [HttpPost]
    public async Task<ActionResult<EscapeRoomDTO>> Create([FromBody] EscapeRoomUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Provjeri postoji li lokacija
        var location = await _context.Locations.FindAsync(dto.LocationId);
        if (location == null)
        {
            return BadRequest(new { message = "Lokacija sa navedenim ID-om ne postoji." });
        }

        // Provjeri postoji li kompanija
        var company = await _context.Companies.FindAsync(dto.CompanyId);
        if (company == null)
        {
            return BadRequest(new { message = "Kompanija sa navedenim ID-om ne postoji." });
        }

        var room = new EscapeRoom
        {
            Name = dto.Name,
            Description = dto.Description,
            Difficulty = (DifficultyLevel)dto.Difficulty,
            MaxPlayers = dto.MaxPlayers,
            Price = dto.Price,
            LocationId = dto.LocationId,
            CompanyId = dto.CompanyId,
            CreatedAt = DateTime.UtcNow,
            DeletedAt = null
        };

        _context.EscapeRooms.Add(room);
        await _context.SaveChangesAsync();

        // Reload sa relacijama za DTO
        var createdRoom = await _context.EscapeRooms
            .Where(r => r.Id == room.Id)
            .Include(r => r.Location)
            .Include(r => r.Company)
            .Include(r => r.Themes)
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, ToDTO(createdRoom));
    }

    /// <summary>
    /// PUT /api/escaperoom/{id} - Ažurira escape room
    /// </summary>
    [Authorize(Roles = "Admin,Editor")]
    [HttpPut("{id}")]
    public async Task<ActionResult<EscapeRoomDTO>> Update(int id, [FromBody] EscapeRoomUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var room = await _context.EscapeRooms
            .Where(r => r.Id == id && r.DeletedAt == null)
            .Include(r => r.Location)
            .Include(r => r.Company)
            .Include(r => r.Themes)
            .FirstOrDefaultAsync();

        if (room == null)
        {
            return NotFound(new { message = $"Escape room sa ID {id} nije pronađen." });
        }

        // Provjeri postoji li lokacija
        var location = await _context.Locations.FindAsync(dto.LocationId);
        if (location == null)
        {
            return BadRequest(new { message = "Lokacija sa navedenim ID-om ne postoji." });
        }

        // Provjeri postoji li kompanija
        var company = await _context.Companies.FindAsync(dto.CompanyId);
        if (company == null)
        {
            return BadRequest(new { message = "Kompanija sa navedenim ID-om ne postoji." });
        }

        // Ručno mapiraj polja
        room.Name = dto.Name;
        room.Description = dto.Description;
        room.Difficulty = (DifficultyLevel)dto.Difficulty;
        room.MaxPlayers = dto.MaxPlayers;
        room.Price = dto.Price;
        room.LocationId = dto.LocationId;
        room.CompanyId = dto.CompanyId;

        _context.EscapeRooms.Update(room);
        await _context.SaveChangesAsync();

        // Reload sa svim relacijama
        var updatedRoom = await _context.EscapeRooms
            .Where(r => r.Id == id)
            .Include(r => r.Location)
            .Include(r => r.Company)
            .Include(r => r.Themes)
            .FirstAsync();

        return Ok(ToDTO(updatedRoom));
    }

    /// <summary>
    /// DELETE /api/escaperoom/{id} - Soft delete escape rooma
    /// </summary>
    [Authorize(Roles = "Admin,Editor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _context.EscapeRooms
            .Where(r => r.Id == id && r.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (room == null)
        {
            return NotFound(new { message = $"Escape room sa ID {id} nije pronađen." });
        }

        room.DeletedAt = DateTime.UtcNow;
        _context.EscapeRooms.Update(room);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

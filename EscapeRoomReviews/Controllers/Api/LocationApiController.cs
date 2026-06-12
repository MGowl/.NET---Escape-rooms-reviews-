using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;

namespace EscapeRoomReviews.Controllers.Api;

[ApiController]
[Route("api/location")]
public class LocationApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LocationApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Mapira Location entitet u LocationDTO.
    /// </summary>
    private static LocationDTO ToDTO(Location location)
    {
        return new LocationDTO
        {
            Id = location.Id,
            City = location.City,
            Address = location.Address,
            PostalCode = location.PostalCode,
            Latitude = location.Latitude,
            Longitude = location.Longitude
        };
    }

    /// <summary>
    /// GET /api/location - Vraća sve aktivne lokacije (gdje je DeletedAt == null).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDTO>>> GetAll()
    {
        var locations = await _context.Locations
            .Where(l => l.DeletedAt == null)
            .ToListAsync();

        return Ok(locations.Select(ToDTO).ToList());
    }

    /// <summary>
    /// GET /api/location/{id} - Vraća lokaciju po ID-u.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDTO>> GetById(int id)
    {
        var location = await _context.Locations
            .Where(l => l.Id == id && l.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (location == null)
        {
            return NotFound();
        }

        return Ok(ToDTO(location));
    }

    /// <summary>
    /// GET /api/location/search/{q} - Pretraga lokacija po gradu i adresi.
    /// </summary>
    [HttpGet("search/{q}")]
    public async Task<ActionResult<IEnumerable<LocationDTO>>> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Pojam za pretragu je obavezan." });
        }

        var locations = await _context.Locations
            .Where(l => l.DeletedAt == null &&
                        (l.City.Contains(q) || l.Address.Contains(q)))
            .ToListAsync();

        return Ok(locations.Select(ToDTO).ToList());
    }

    /// <summary>
    /// POST /api/location - Kreira novu lokaciju.
    /// </summary>
    [Authorize(Roles = "Admin,Editor")]
    [HttpPost]
    public async Task<ActionResult<LocationDTO>> Create([FromBody] LocationUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var location = new Location
        {
            City = dto.City,
            Address = dto.Address,
            PostalCode = dto.PostalCode,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            DeletedAt = null
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = location.Id }, ToDTO(location));
    }

    /// <summary>
    /// PUT /api/location/{id} - Ažurira lokaciju po ID-u.
    /// </summary>
    [Authorize(Roles = "Admin,Editor")]
    [HttpPut("{id}")]
    public async Task<ActionResult<LocationDTO>> Update(int id, [FromBody] LocationUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var location = await _context.Locations
            .Where(l => l.Id == id && l.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (location == null)
        {
            return NotFound();
        }

        location.City = dto.City;
        location.Address = dto.Address;
        location.PostalCode = dto.PostalCode;
        location.Latitude = dto.Latitude;
        location.Longitude = dto.Longitude;

        _context.Locations.Update(location);
        await _context.SaveChangesAsync();

        return Ok(ToDTO(location));
    }

    /// <summary>
    /// DELETE /api/location/{id} - Soft delete lokacije.
    /// </summary>
    [Authorize(Roles = "Admin,Editor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _context.Locations
            .Where(l => l.Id == id && l.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (location == null)
        {
            return NotFound();
        }

        location.DeletedAt = DateTime.UtcNow;
        _context.Locations.Update(location);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
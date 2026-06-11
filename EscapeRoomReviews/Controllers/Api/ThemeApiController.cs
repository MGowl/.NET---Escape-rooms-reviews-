using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;

namespace EscapeRoomReviews.Controllers.Api;

[ApiController]
[Route("api/theme")]
public class ThemeApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ThemeApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Mapira Theme entitet u ThemeDTO.
    /// </summary>
    private static ThemeDTO ToDTO(Theme theme)
    {
        return new ThemeDTO
        {
            Id = theme.Id,
            Name = theme.Name,
            IconUrl = theme.IconUrl
        };
    }

    /// <summary>
    /// GET /api/theme - Vraća sve aktivne teme (gdje je DeletedAt == null).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ThemeDTO>>> GetAll()
    {
        var themes = await _context.Themes
            .Where(t => t.DeletedAt == null)
            .ToListAsync();

        return Ok(themes.Select(ToDTO).ToList());
    }

    /// <summary>
    /// GET /api/theme/{id} - Vraća temu po ID-u.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ThemeDTO>> GetById(int id)
    {
        var theme = await _context.Themes
            .Where(t => t.Id == id && t.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (theme == null)
        {
            return NotFound();
        }

        return Ok(ToDTO(theme));
    }

    /// <summary>
    /// GET /api/theme/search/{q} - Pretraga tema po nazivu.
    /// </summary>
    [HttpGet("search/{q}")]
    public async Task<ActionResult<IEnumerable<ThemeDTO>>> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Pojam za pretragu je obavezan." });
        }

        var themes = await _context.Themes
            .Where(t => t.DeletedAt == null && t.Name.Contains(q))
            .ToListAsync();

        return Ok(themes.Select(ToDTO).ToList());
    }

    /// <summary>
    /// POST /api/theme - Kreira novu temu.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ThemeDTO>> Create([FromBody] ThemeUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var theme = new Theme
        {
            Name = dto.Name,
            IconUrl = dto.IconUrl,
            DeletedAt = null
        };

        _context.Themes.Add(theme);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = theme.Id }, ToDTO(theme));
    }

    /// <summary>
    /// PUT /api/theme/{id} - Ažurira temu po ID-u.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ThemeDTO>> Update(int id, [FromBody] ThemeUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var theme = await _context.Themes
            .Where(t => t.Id == id && t.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (theme == null)
        {
            return NotFound();
        }

        theme.Name = dto.Name;
        theme.IconUrl = dto.IconUrl;

        _context.Themes.Update(theme);
        await _context.SaveChangesAsync();

        return Ok(ToDTO(theme));
    }

    /// <summary>
    /// DELETE /api/theme/{id} - Soft delete teme.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var theme = await _context.Themes
            .Where(t => t.Id == id && t.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (theme == null)
        {
            return NotFound();
        }

        theme.DeletedAt = DateTime.UtcNow;
        _context.Themes.Update(theme);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
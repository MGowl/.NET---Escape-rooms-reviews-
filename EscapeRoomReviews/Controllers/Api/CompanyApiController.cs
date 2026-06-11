using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscapeRoomReviews.Data;
using EscapeRoomReviews.Models.Domain;
using EscapeRoomReviews.Models.DTOs;

namespace EscapeRoomReviews.Controllers.Api;

[ApiController]
[Route("api/company")]
public class CompanyApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CompanyApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Mapira Company entitet u CompanyDTO.
    /// </summary>
    private static CompanyDTO ToDTO(Company company)
    {
        return new CompanyDTO
        {
            Id = company.Id,
            Name = company.Name,
            Website = company.Website
        };
    }

    /// <summary>
    /// GET /api/company - Vraća sve aktivne kompanije (gdje je DeletedAt == null).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetAll()
    {
        var companies = await _context.Companies
            .Where(c => c.DeletedAt == null)
            .ToListAsync();

        return Ok(companies.Select(ToDTO).ToList());
    }

    /// <summary>
    /// GET /api/company/{id} - Vraća kompaniju po ID-u.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDTO>> GetById(int id)
    {
        var company = await _context.Companies
            .Where(c => c.Id == id && c.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (company == null)
        {
            return NotFound();
        }

        return Ok(ToDTO(company));
    }

    /// <summary>
    /// GET /api/company/search/{q} - Pretraga kompanija po nazivu.
    /// </summary>
    [HttpGet("search/{q}")]
    public async Task<ActionResult<IEnumerable<CompanyDTO>>> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Pojam za pretragu je obavezan." });
        }

        var companies = await _context.Companies
            .Where(c => c.DeletedAt == null && c.Name.Contains(q))
            .ToListAsync();

        return Ok(companies.Select(ToDTO).ToList());
    }

    /// <summary>
    /// POST /api/company - Kreira novu kompaniju.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CompanyDTO>> Create([FromBody] CompanyUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var company = new Company
        {
            Name = dto.Name,
            Website = dto.Website,
            DeletedAt = null
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = company.Id }, ToDTO(company));
    }

    /// <summary>
    /// PUT /api/company/{id} - Ažurira kompaniju po ID-u.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CompanyDTO>> Update(int id, [FromBody] CompanyUpsertDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var company = await _context.Companies
            .Where(c => c.Id == id && c.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (company == null)
        {
            return NotFound();
        }

        company.Name = dto.Name;
        company.Website = dto.Website;

        _context.Companies.Update(company);
        await _context.SaveChangesAsync();

        return Ok(ToDTO(company));
    }

    /// <summary>
    /// DELETE /api/company/{id} - Soft delete kompanije.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var company = await _context.Companies
            .Where(c => c.Id == id && c.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (company == null)
        {
            return NotFound();
        }

        company.DeletedAt = DateTime.UtcNow;
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
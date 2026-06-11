using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.DTOs;

public class CompanyUpsertDTO
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 200 znakova.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Web stranica može imati maksimalno 500 znakova.")]
    public string Website { get; set; } = string.Empty;
}
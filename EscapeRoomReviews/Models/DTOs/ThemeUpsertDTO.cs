using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.DTOs;

public class ThemeUpsertDTO
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 100 znakova.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Icon URL može imati maksimalno 1000 znakova.")]
    public string IconUrl { get; set; } = string.Empty;
}
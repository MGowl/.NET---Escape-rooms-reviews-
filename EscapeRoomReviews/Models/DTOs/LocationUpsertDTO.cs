using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.DTOs;

public class LocationUpsertDTO
{
    [Required(ErrorMessage = "Grad je obavezan.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Grad mora imati između 2 i 100 znakova.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adresa je obavezna.")]
    [StringLength(250, MinimumLength = 3, ErrorMessage = "Adresa mora imati između 3 i 250 znakova.")]
    public string Address { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Poštanski broj može imati maksimalno 20 znakova.")]
    public string PostalCode { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}
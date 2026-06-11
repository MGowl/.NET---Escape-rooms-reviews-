using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.DTOs;

public class EscapeRoomUpsertDTO
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Naziv mora imati između 3 i 200 znakova.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Opis može imati maksimalno 1000 znakova.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Težina je obavezna.")]
    public int Difficulty { get; set; }

    [Required(ErrorMessage = "Broj igrača je obavezan.")]
    [Range(1, int.MaxValue, ErrorMessage = "Broj igrača mora biti veći od 0.")]
    public int MaxPlayers { get; set; }

    [Required(ErrorMessage = "Cijena je obavezna.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti veća od 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Lokacija je obavezna.")]
    [Range(1, int.MaxValue, ErrorMessage = "Odaberite lokaciju.")]
    public int LocationId { get; set; }

    [Required(ErrorMessage = "Kompanija je obavezna.")]
    [Range(1, int.MaxValue, ErrorMessage = "Odaberite kompaniju.")]
    public int CompanyId { get; set; }
}

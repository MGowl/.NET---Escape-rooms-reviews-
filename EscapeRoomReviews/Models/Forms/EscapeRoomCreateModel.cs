using System.ComponentModel.DataAnnotations;
using EscapeRoomReviews.Models.Domain;

namespace EscapeRoomReviews.Models.Forms;

public class EscapeRoomCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati izmedu 2 i 100 znakova.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis je obavezan.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Opis mora imati izmedu 10 i 2000 znakova.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tezina je obavezna.")]
    [EnumDataType(typeof(DifficultyLevel), ErrorMessage = "Odaberite vazecu tezinu.")]
    public DifficultyLevel Difficulty { get; set; }

    [Required(ErrorMessage = "Max igraca je obavezan.")]
    [Range(1, int.MaxValue, ErrorMessage = "Max igraca mora biti veci od 0.")]
    public int MaxPlayers { get; set; }

    [Required(ErrorMessage = "Cijena je obavezna.")]
    [Range(typeof(decimal), "0", "100000", ErrorMessage = "Cijena mora biti 0 ili veca.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Lokacija je obavezna.")]
    [Range(1, int.MaxValue, ErrorMessage = "Odaberite lokaciju.")]
    public int LocationId { get; set; }

    [Required(ErrorMessage = "Tvrtka je obavezna.")]
    [Range(1, int.MaxValue, ErrorMessage = "Odaberite tvrtku.")]
    public int CompanyId { get; set; }

    [MinLength(1, ErrorMessage = "Odaberite barem jednu temu.")]
    public List<int> ThemeIds { get; set; } = new();
}

using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.Forms;

public class ThemeEditModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati izmedu 2 i 100 znakova.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Icon URL je obavezan.")]
    [Url(ErrorMessage = "Unesite ispravan URL.")]
    public string IconUrl { get; set; } = string.Empty;
}

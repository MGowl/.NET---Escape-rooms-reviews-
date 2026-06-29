using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.Forms;

public class ThemeCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati izmedu 2 i 100 znakova.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Icon URL je obavezan.")]
    [RegularExpression(@"^bi bi-[a-z0-9\-]+$", ErrorMessage = "Format mora biti 'bi bi-nazivikone', npr. 'bi bi-search'.")]
    public string IconUrl { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.Forms;

public class CompanyCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati izmedu 2 i 100 znakova.")]
    public string Name { get; set; } = string.Empty;

    [Url(ErrorMessage = "Unesite ispravan URL.")]
    public string Website { get; set; } = string.Empty;
}

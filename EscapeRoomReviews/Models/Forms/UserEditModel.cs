using System.ComponentModel.DataAnnotations;
using EscapeRoomReviews.Models.Domain;

namespace EscapeRoomReviews.Models.Forms;

public class UserEditModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Korisnicko ime je obavezno.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisnicko ime mora imati izmedu 3 i 50 znakova.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Unesite ispravan email.")]
    public string Email { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Odigrane sobe moraju biti 0 ili vise.")]
    public int TotalRoomsPlayed { get; set; }

    [Required(ErrorMessage = "Uloga je obavezna.")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Odaberite vazecu ulogu.")]
    public UserRole Role { get; set; }
}

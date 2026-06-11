using System.ComponentModel.DataAnnotations;
using EscapeRoomReviews.Models.Domain;

namespace EscapeRoomReviews.Models.DTOs;

public class UserUpsertDTO
{
    [Required(ErrorMessage = "Korisničko ime je obavezno.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 100 znakova.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Email nije u validnom formatu.")]
    [StringLength(200, ErrorMessage = "Email može imati maksimalno 200 znakova.")]
    public string Email { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Ukupan broj odigranih soba ne može biti negativan.")]
    public int TotalRoomsPlayed { get; set; }

    [Required(ErrorMessage = "Uloga je obavezna.")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Odaberite važeću ulogu.")]
    public UserRole Role { get; set; }
}
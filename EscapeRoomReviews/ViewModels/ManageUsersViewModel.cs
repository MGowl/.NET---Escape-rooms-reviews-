using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.ViewModels;

public class ManageUsersViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ime je obavezno.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Unesite ispravan email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Uloga je obavezna.")]
    public string Role { get; set; } = string.Empty;

    public int ReviewCount { get; set; }
}

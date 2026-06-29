using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EscapeRoomReviews.Models.Domain;

public class AppUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
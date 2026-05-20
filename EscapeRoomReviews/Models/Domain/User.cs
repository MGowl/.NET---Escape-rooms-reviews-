using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscapeRoomReviews.Models.Domain;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public DateTime RegisteredAt { get; set; }

    public int TotalRoomsPlayed { get; set; }

    public UserRole Role { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Reviews created by this user.
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

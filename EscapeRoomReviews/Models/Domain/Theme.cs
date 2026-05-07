using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscapeRoomReviews.Models.Domain;

[Table("Themes")]
public class Theme
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string IconUrl { get; set; } = string.Empty;

    // Rooms that use this theme.
    public virtual ICollection<EscapeRoom> EscapeRooms { get; set; } = new List<EscapeRoom>();
}

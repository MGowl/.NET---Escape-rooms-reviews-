using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscapeRoomReviews.Models.Domain;

[Table("EscapeRooms")]
public class EscapeRoom
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DifficultyLevel Difficulty { get; set; }

    public int MaxPlayers { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    // FK + navigation to the physical location where the room is hosted.
    [ForeignKey(nameof(Location))]
    public int LocationId { get; set; }
    public virtual Location Location { get; set; } = null!;

    // FK + navigation to the company that owns this room.
    [ForeignKey(nameof(Company))]
    public int CompanyId { get; set; }
    public virtual Company Company { get; set; } = null!;

    // Related user content and metadata for this room.
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public virtual ICollection<Theme> Themes { get; set; } = new List<Theme>();
}

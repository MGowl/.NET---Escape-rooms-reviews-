namespace EscapeRoomReviews.Models.Domain;

/// <summary>
/// Represents a single escape room listing in the system.
/// </summary>
public class EscapeRoom
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public int MaxPlayers { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    // FK + navigation to the physical location where the room is hosted.
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    // FK + navigation to the company that owns this room.
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    // Related user content and metadata for this room.
    public List<Review> Reviews { get; set; } = new();
    public List<Photo> Photos { get; set; } = new();
    public List<Theme> Themes { get; set; } = new();
}

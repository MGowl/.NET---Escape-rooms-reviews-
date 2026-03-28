namespace EscapeRoomReviews.Models.Domain;

public class EscapeRoom
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public int MaxPlayers { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public List<Review> Reviews { get; set; } = new();
    public List<Photo> Photos { get; set; } = new();
    public List<Theme> Themes { get; set; } = new();
}

using EscapeRoomReviews.Models.Domain;

namespace EscapeRoomReviews.Models.Forms;

public class EscapeRoomCreateModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public int MaxPlayers { get; set; }
    public decimal Price { get; set; }
    public int LocationId { get; set; }
    public int CompanyId { get; set; }
}

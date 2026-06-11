namespace EscapeRoomReviews.Models.DTOs;

public class EscapeRoomDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public int MaxPlayers { get; set; }
    public decimal Price { get; set; }
    public LocationDTO Location { get; set; } = null!;
    public CompanyDTO Company { get; set; } = null!;
    public List<ThemeDTO> Themes { get; set; } = new();
}

namespace EscapeRoomReviews.Models.Domain;

public class Theme
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;

    public List<EscapeRoom> EscapeRooms { get; set; } = new();
}

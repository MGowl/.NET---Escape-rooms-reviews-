namespace EscapeRoomReviews.Models.Domain;

/// <summary>
/// Represents a theme tag that can be assigned to escape rooms.
/// </summary>
public class Theme
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;

    // Rooms that use this theme.
    public List<EscapeRoom> EscapeRooms { get; set; } = new();
}

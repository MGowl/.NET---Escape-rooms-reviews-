namespace EscapeRoomReviews.Models.Domain;

/// <summary>
/// Represents a company that owns and manages escape rooms.
/// </summary>
public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    // Navigation collection for all rooms owned by this company.
    public List<EscapeRoom> EscapeRooms { get; set; } = new();
}

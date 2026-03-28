namespace EscapeRoomReviews.Models.Domain;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public List<EscapeRoom> EscapeRooms { get; set; } = new();
}

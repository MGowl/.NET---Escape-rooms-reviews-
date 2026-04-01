namespace EscapeRoomReviews.Models.Domain;

/// <summary>
/// Represents a geographic location used by one or more escape rooms.
/// </summary>
public class Location
{
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Rooms available at this location.
    public List<EscapeRoom> EscapeRooms { get; set; } = new();
}

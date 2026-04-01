namespace EscapeRoomReviews.Models.Domain;

/// <summary>
/// Stores a photo URL associated with an escape room.
/// </summary>
public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;

    // FK + navigation to the room this photo belongs to.
    public int EscapeRoomId { get; set; }
    public EscapeRoom EscapeRoom { get; set; } = null!;
}

namespace EscapeRoomReviews.Models.Domain;

public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;

    public int EscapeRoomId { get; set; }
    public EscapeRoom EscapeRoom { get; set; } = null!;
}

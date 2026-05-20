namespace EscapeRoomReviews.Models.Forms;

public class ReviewCreateModel
{
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int PlayersCount { get; set; }
    public int EscapeRoomId { get; set; }
}

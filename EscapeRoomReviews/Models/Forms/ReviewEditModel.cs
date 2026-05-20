namespace EscapeRoomReviews.Models.Forms;

public class ReviewEditModel
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int PlayersCount { get; set; }
    public int EscapeRoomId { get; set; }
    public bool IsVerified { get; set; }
}

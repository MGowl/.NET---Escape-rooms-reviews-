namespace EscapeRoomReviews.ViewModels;

public class MyReviewViewModel
{
    public int Id { get; set; }
    public string EscapeRoomName { get; set; } = string.Empty;
    public int EscapeRoomId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

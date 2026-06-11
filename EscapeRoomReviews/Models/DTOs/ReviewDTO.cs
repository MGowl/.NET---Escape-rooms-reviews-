namespace EscapeRoomReviews.Models.DTOs;

public class ReviewDTO
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int PlayersCount { get; set; }
    public DateTime? VisitedAt { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public string EscapeRoomName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
namespace EscapeRoomReviews.Models.Domain;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsVerified { get; set; }
    public int PlayersCount { get; set; }

    public int EscapeRoomId { get; set; }
    public EscapeRoom EscapeRoom { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

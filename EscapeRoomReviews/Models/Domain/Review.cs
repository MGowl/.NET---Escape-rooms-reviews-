namespace EscapeRoomReviews.Models.Domain;

/// <summary>
/// Represents a user review and rating for a specific escape room.
/// </summary>
public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsVerified { get; set; }
    public int PlayersCount { get; set; }

    // FK + navigation to the reviewed room.
    public int EscapeRoomId { get; set; }
    public EscapeRoom EscapeRoom { get; set; } = null!;

    // FK + navigation to the author of the review.
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

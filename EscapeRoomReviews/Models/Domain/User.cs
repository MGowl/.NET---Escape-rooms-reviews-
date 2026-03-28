namespace EscapeRoomReviews.Models.Domain;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public int TotalRoomsPlayed { get; set; }
    public UserRole Role { get; set; }

    public List<Review> Reviews { get; set; } = new();
}

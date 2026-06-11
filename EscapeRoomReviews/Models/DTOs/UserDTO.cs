namespace EscapeRoomReviews.Models.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalRoomsPlayed { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}
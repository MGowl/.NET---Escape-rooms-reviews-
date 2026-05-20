using EscapeRoomReviews.Models.Domain;

namespace EscapeRoomReviews.Models.Forms;

public class UserCreateModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalRoomsPlayed { get; set; }
    public UserRole Role { get; set; }
}

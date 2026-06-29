using EscapeRoomReviews.Models.Domain;

namespace EscapeRoomReviews.ViewModels;

public class UserDetailsViewModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int TotalRoomsPlayed { get; set; }
    public DateTime RegisteredAt { get; set; }
    public List<Review> Reviews { get; set; } = new();
}

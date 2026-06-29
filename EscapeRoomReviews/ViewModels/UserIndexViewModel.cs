using System;

namespace EscapeRoomReviews.ViewModels
{
    public class UserIndexViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int TotalRoomsPlayed { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}

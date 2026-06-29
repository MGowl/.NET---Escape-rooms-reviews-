using System;

namespace EscapeRoomReviews.ViewModels
{
    public class ReviewIndexViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }
    }
}

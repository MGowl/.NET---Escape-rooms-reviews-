namespace EscapeRoomReviews.ViewModels
{
    public class LocationIndexViewModel
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public int RoomCount { get; set; }
    }
}

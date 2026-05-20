namespace EscapeRoomReviews.Models.Forms;

public class LocationEditModel
{
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

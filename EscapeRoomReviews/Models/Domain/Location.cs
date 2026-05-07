using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscapeRoomReviews.Models.Domain;

[Table("Locations")]
public class Location
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    // Rooms available at this location.
    public virtual ICollection<EscapeRoom> EscapeRooms { get; set; } = new List<EscapeRoom>();
}

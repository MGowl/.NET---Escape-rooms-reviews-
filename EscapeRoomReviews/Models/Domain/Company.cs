using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscapeRoomReviews.Models.Domain;

[Table("Companies")]
public class Company
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Website { get; set; } = string.Empty;

    // Navigation collection for all rooms owned by this company.
    public virtual ICollection<EscapeRoom> EscapeRooms { get; set; } = new List<EscapeRoom>();
}

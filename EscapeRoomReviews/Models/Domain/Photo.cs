using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscapeRoomReviews.Models.Domain;

[Table("Photos")]
public class Photo
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Url { get; set; } = string.Empty;

    // FK + navigation to the room this photo belongs to.
    [ForeignKey(nameof(EscapeRoom))]
    public int EscapeRoomId { get; set; }
    public virtual EscapeRoom EscapeRoom { get; set; } = null!;
}

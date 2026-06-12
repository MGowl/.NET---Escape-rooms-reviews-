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

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime CreatedAt { get; set; }

    // FK + navigation to the room this photo belongs to.
    [ForeignKey(nameof(EscapeRoom))]
    public int EscapeRoomId { get; set; }
    public virtual EscapeRoom EscapeRoom { get; set; } = null!;
}

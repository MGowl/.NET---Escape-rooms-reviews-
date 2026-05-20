using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscapeRoomReviews.Models.Domain;

[Table("Reviews")]
public class Review
{
    [Key]
    public int Id { get; set; }

    [Range(1,5)]
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool IsVerified { get; set; }

    public int PlayersCount { get; set; }

    // FK + navigation to the reviewed room.
    [ForeignKey(nameof(EscapeRoom))]
    public int EscapeRoomId { get; set; }
    public virtual EscapeRoom EscapeRoom { get; set; } = null!;

    // FK + navigation to the author of the review.
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
}

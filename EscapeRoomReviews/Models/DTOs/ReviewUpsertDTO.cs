using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.DTOs;

public class ReviewUpsertDTO
{
    [Range(1, 5, ErrorMessage = "Ocjena mora biti između 1 i 5.")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Komentar je obavezan.")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Komentar mora imati između 5 i 1000 znakova.")]
    public string Comment { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Broj igrača mora biti veći od 0.")]
    public int PlayersCount { get; set; }

    public DateTime? VisitedAt { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Escape room je obavezan.")]
    public int EscapeRoomId { get; set; }

    public string? AppUserId { get; set; }

    public bool IsVerified { get; set; }
}
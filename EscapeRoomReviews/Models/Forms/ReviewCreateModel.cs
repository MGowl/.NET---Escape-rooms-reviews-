using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.Forms;

public class ReviewCreateModel
{
    [Required(ErrorMessage = "Ocjena je obavezna.")]
    [Range(1, 5, ErrorMessage = "Ocjena mora biti izmedu 1 i 5.")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Komentar je obavezan.")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Komentar mora imati izmedu 5 i 1000 znakova.")]
    public string Comment { get; set; } = string.Empty;

    [Required(ErrorMessage = "Broj igraca je obavezan.")]
    [Range(1, int.MaxValue, ErrorMessage = "Broj igraca mora biti veci od 0.")]
    public int PlayersCount { get; set; }

    [Required(ErrorMessage = "Datum posjeta je obavezan.")]
    public DateTime? VisitedAt { get; set; }

    [Required(ErrorMessage = "Escape room je obavezan.")]
    [Range(1, int.MaxValue, ErrorMessage = "Odaberite escape room.")]
    public int EscapeRoomId { get; set; }
}

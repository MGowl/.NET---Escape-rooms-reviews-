using System.ComponentModel.DataAnnotations;

namespace EscapeRoomReviews.Models.Forms;

public class LocationEditModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Grad je obavezan.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Grad mora imati izmedu 2 i 100 znakova.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adresa je obavezna.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Adresa mora imati izmedu 2 i 200 znakova.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postanski broj je obavezan.")]
    [StringLength(10, MinimumLength = 4, ErrorMessage = "Postanski broj mora imati izmedu 4 i 10 znakova.")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Latitude je obavezna.")]
    [Range(-90, 90, ErrorMessage = "Latitude mora biti izmedu -90 i 90.")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "Longitude je obavezna.")]
    [Range(-180, 180, ErrorMessage = "Longitude mora biti izmedu -180 i 180.")]
    public double Longitude { get; set; }
}

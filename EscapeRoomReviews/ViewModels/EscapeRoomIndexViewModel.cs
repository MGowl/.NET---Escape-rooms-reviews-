using System;
using System.Collections.Generic;

namespace EscapeRoomReviews.ViewModels
{
    public class EscapeRoomIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string City { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ThemeSlimViewModel> Themes { get; set; } = new();
        public string? FirstPhotoUrl { get; set; }
    }
}

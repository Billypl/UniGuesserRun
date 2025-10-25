using System.ComponentModel.DataAnnotations;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Application.Models.PlaceModels
{
    public class NewPlaceDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Coordinates Coordinates { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public string Alt { get; set; } 
        [Required]
        public string Difficulty { get; set; }
    }
}

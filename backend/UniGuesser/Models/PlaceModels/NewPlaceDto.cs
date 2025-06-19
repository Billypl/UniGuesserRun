using PartyGame.Models.GameModels;
using System.ComponentModel.DataAnnotations;

namespace PartyGame.Models.PlaceModels
{
    public class NewPlaceDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Coordinates Coordinates { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string Alt { get; set; }
        [Required]
        public string Difficulty { get; set; }
    }
}

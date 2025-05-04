using PartyGame.Models.GameModels;

namespace PartyGame.Models.PlaceModels
{
    public class ShowPlaceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Coordinates Coordinates { get; set; }
        public string ImageUrl { get; set; }
        public string Alt { get; set; }
        public string DifficultyLevel { get; set; }

        public string? AuthorId { get; set; }
        public string? AuthorName { get; set; }
    }
}

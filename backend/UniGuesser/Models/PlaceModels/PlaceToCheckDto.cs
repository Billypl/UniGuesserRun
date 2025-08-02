
namespace PartyGame.Models.PlaceModels
{
    public class PlaceToCheckDto
    {
        public string Id { get; set; }
        public NewPlaceDto NewPlace { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

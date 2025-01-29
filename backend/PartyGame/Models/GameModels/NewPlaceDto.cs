namespace PartyGame.Models.GameModels
{
    public class NewPlaceDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Coordinates Coordinates { get; set; }
        public string ImageUrl { get; set; }
        public string Alt { get; set; }
    }
}

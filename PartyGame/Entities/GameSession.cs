namespace PartyGame.Entities
{
    public class GameSession
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public Place Place { get; set; }
    }
}

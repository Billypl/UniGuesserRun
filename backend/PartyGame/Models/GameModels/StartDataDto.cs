namespace PartyGame.Models.GameModels
{
    public class StartDataDto
    {
        // when user is logged nickname is not required 
        public string? Nickname { get; set; }
        public string Difficulty { get; set; }
    }
}



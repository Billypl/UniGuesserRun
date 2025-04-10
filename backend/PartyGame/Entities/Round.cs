using PartyGame.Models.GameModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PartyGame.Entities
{
    public class Round
    {
        [Key]
        public int Id { get; set; }
        public Guid PublicId { get; set; } = Guid.NewGuid();
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Score { get; set; }

        public int GameSessionId { get; set; }
        [JsonIgnore]
        public virtual GameSession GameSession { get; set; }

        public int PlaceId { get; set; }
        public virtual Place PlaceToGuess { get; set; }

    }
}

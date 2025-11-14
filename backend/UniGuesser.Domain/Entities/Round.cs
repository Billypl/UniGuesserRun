using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UniGuesser.Domain.Entities;

public class Round
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Score { get; set; }
    public double Distance { get; set; }
    public Guid GameSessionId { get; set; }

    [JsonIgnore] public virtual GameSession GameSession { get; set; }

    public Guid PlaceId { get; set; }
    public virtual Place PlaceToGuess { get; set; }
}
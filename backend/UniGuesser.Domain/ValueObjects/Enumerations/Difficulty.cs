using System.Text.Json.Serialization;

namespace UniGuesser.Domain.ValueObjects.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DifficultyLevel
{
    [JsonPropertyName("Easy")]
    Easy,

    [JsonPropertyName("Normal")]
    Normal,

    [JsonPropertyName("Hard")]
    Hard
}
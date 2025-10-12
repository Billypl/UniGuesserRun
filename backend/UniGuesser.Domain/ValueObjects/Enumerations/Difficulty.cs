using System.Text.Json.Serialization;

namespace UniGuesser.Domain.ValueObjects.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DifficultyLevel
{
    [JsonPropertyName("easy")]
    easy,

    [JsonPropertyName("normal")]
    normal,

    [JsonPropertyName("hard")]
    hard
}
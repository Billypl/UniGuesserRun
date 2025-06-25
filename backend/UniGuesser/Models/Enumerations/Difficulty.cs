using System.Text.Json.Serialization;

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
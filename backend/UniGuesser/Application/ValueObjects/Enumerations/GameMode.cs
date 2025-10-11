using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UniGuesser.Application.Models.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameMode
    {
        [EnumMember(Value = "classic")]
        Classic,

        [EnumMember(Value = "geolocation")]
        Geolocation
    }
}
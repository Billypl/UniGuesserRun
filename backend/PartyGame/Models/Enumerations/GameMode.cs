using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PartyGame.Models
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
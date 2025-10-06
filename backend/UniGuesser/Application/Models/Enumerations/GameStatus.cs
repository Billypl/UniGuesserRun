using System.Runtime.Serialization;

namespace UniGuesser.Application.Models.Enumerations
{
    public enum GameStatus
    {
        [EnumMember(Value = "inprogress")]
        InProgress,

        [EnumMember(Value = "finished")]
        Finished,

        [EnumMember(Value = "abandoned")]
        Abandoned,
    }
}

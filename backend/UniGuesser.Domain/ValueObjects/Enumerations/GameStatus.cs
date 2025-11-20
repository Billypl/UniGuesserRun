using System.Runtime.Serialization;

namespace UniGuesser.Domain.ValueObjects.Enumerations;

public enum GameStatus
{
    [EnumMember(Value = "inprogress")] InProgress,

    [EnumMember(Value = "finished")] Finished,

    [EnumMember(Value = "abandoned")] Abandoned,

    [EnumMember(Value = "toDelete")] ToDelete
}
using System.Runtime.Serialization;

namespace UniGuesser.Domain.ValueObjects.Enumerations;

public enum ImageType
{
    [EnumMember(Value = "file")] File,
    [EnumMember(Value = "url")] Url
}
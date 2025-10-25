using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UniGuesser.Domain.ValueObjects.Enumerations
{
    public enum ImageType
    {
        [EnumMember(Value = "local")]
        Local,
        [EnumMember(Value = "remote")]
        Remote
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CommonDTOs
{
    [DataContract(Name = "Result")]
    public class Result
    {
        [DataMember(Name = "Res")]
        public bool Res { get; set; }

        [DataMember(Name = "ResultMessage")]
        public object ResultMessage { get; set; }
    }
}

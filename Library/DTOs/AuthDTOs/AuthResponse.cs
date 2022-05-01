using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AuthDTOs
{
    [DataContract(Name = "AuthResponse")]
    public class AuthResponse
    {
        [DataMember(Name = "Token")]
        public string Token { get; set; }

        [DataMember(Name = "RefreshToken")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "IsSuccess")]
        public bool IsSuccess { get; set; }

        [DataMember(Name = "Reason")]
        public string Reason { get; set; }
    }
}

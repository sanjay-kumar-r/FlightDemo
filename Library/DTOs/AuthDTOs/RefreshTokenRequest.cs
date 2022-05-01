using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace AuthDTOs
{
    [DataContract(Name = "RefreshTokenRequest")]
    public class RefreshTokenRequest
    {
        [DataMember(Name = "ExpiredToken")]
        [Required]
        public string ExpiredToken { get; set; }

        [DataMember(Name = "RefreshToken")]
        [Required]
        public string RefreshToken { get; set; }
    }
}

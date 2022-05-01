using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace AuthDTOs
{
    [DataContract(Name = "AuthRequest")]
    public class AuthRequest
    {
        [DataMember(Name = "UserName")]
        [Required]
        public string UserName { get; set; }

        [DataMember(Name = "Password")]
        [Required]
        public string Password { get; set; }
    }
}

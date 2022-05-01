using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace UsersDTOs
{
    [DataContract(Name = "UserRefreshTokens")]
    //[Table("UserRefreshTokens")]
    public class UserRefreshTokens
    {
        [DataMember(Name = "Id")]
        [Key]
        public long Id { get; set; }

        [DataMember(Name = "Token")]
        public string Token { get; set; }

        [DataMember(Name = "RefreshToken")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = "ExpirationDate")]
        public DateTime ExpirationDate { get; set; }

        [DataMember(Name = "IsActive")]
        [NotMapped]
        public bool IsActive
        {
            get
            {
                return ExpirationDate > DateTime.UtcNow;
            }
            //private set
            //{

            //}
        }

        [DataMember(Name = "IpAddress")]
        public string IpAddress { get; set; }

        [DataMember(Name = "IsInvalidated")]
        public bool IsInvalidated { get; set; }

        [DataMember(Name = "UserId")]
        [ForeignKey("User")]
        public long UserId { get; set; }
        [DataMember(Name = "User")]
        public virtual UsersDTOs.Users User { get; set; }
    }
}

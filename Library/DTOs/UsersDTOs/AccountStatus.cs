using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace UsersDTOs
{
    [DataContract(Name = "AccountStatus")]
    public class AccountStatus
    {
        [DataMember(Name = "Id")]
        //[Key]
        public int Id { get; set; }

        [DataMember(Name = "Status")]
        [Required]
        [MaxLength(512)]
        public string Status { get; set; }

        [DataMember(Name = "Description")]
        [MaxLength(1024)]
        public string Description { get; set; }
    }

    public enum AccountStatusCode
    {
        Registered = 1,
        Active,
        InActive,
        Blocked
        //,Deleted
    }

    public static class AccountStatusDescription
    {
        private static readonly Dictionary<string, string> _accountStatusDescriptions;
        public static Dictionary<string, string> accountStatusDescriptions
        {
            get
            {
                return _accountStatusDescriptions;
            }
        }
        static AccountStatusDescription()
        {
            _accountStatusDescriptions = new Dictionary<string, string>()
            {
                { "REGISTERED", "On user first time register" },
                { "ACTIVE", "On user first time login and there after" },
                { "INACTIVE", "On user not loggedIn long time or updated by admin" },
                { "BLOCKED", "On user invalid/wrong attempt to login or update by admin" }
                //{ "DELETED", "Updated by admin" }
            };
        }

    }
}

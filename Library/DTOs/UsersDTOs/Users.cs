using AuthDTOs;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace UsersDTOs
{
    [DataContract(Name = "Users")]
    public class Users
    {
        [DataMember(Name = "Id")]
        //[Key]
        public long Id { get; set; }

        [DataMember(Name = "FirstName")]
        [MaxLength(512)]
        public string FirstName { get; set; }

        [DataMember(Name = "LastName")]
        [MaxLength(512)]
        public string LastName { get; set; }

        [DataMember(Name = "EmailId")]
        //[Required]
        [MaxLength(1024)]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string EmailId { get; set; }

        [DataMember(Name = "Password")]
        //[Required]
        [MaxLength(1024)]
        [MinLength(5)]
        public string Password { get; set; }

        [DataMember(Name = "AccountStatusId")]
        [ForeignKey("AccountStatus")]
        public virtual int AccountStatusId { get; set; }

        public virtual AccountStatus AccountStatus { get; set; }

        [DataMember(Name = "IsSuperAdmin")]
        public bool IsSuperAdmin { get; set; } = false;

        [DataMember(Name = "CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [DataMember(Name = "ModifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        [DataMember(Name = "IsDeleted")]
        public bool IsDeleted { get; set; } = false;
    }

    [DataContract(Name = "UserDetails")]
    public class UserDetails
    {
        [DataMember(Name = "Id")]
        public long Id { get; set; }

        [DataMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "LastName")]
        public string LastName { get; set; }

        [DataMember(Name = "EmailId")]
        public string EmailId { get; set; }

        [DataMember(Name = "AccountStatusId")]
        public int? AccountStatusId { get; set; }

        [DataMember(Name = "IsSuperAdmin")]
        public bool? IsSuperAdmin { get; set; }
    }

    [DataContract(Name = "UserLoginResponse")]
    public class UserLoginResponse
    {
        [DataMember(Name = "User")]
        public Users User { get; set; }

        [DataMember(Name = "AuthResponse")]
        public AuthResponse AuthResponse { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace UserDtOs
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
        [Key]
        public long Id { get; set; }

        [DataMember(Name = "FirstName")]
        [MaxLength(512)]
        public string FirstName { get; set; }

        [DataMember(Name = "LastName")]
        [MaxLength(512)]
        public string LastName { get; set; }

        [DataMember(Name = "EmailId")]
        [Required]
        [MaxLength(1024)]
        public string EmailId { get; set; }

        [DataMember(Name = "AccountStatus")]
        [Required]
        public AccountStatus AccountStatus { get; set; }

        [DataMember(Name = "IsSuperAdmin")]
        public bool IsSuperAdmin { get; set; } = false;
    }
}

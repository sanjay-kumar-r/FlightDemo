using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace AirlinesDTOs
{
    [DataContract(Name = "DiscountTags")]
    public class DiscountTags
    {
        [DataMember(Name = "Id")]
        //[key]
        public long Id { get; set; }

        [DataMember(Name = "Name")]
        [MaxLength(512)]
        public string Name { get; set; }

        [DataMember(Name = "DiscountCode")]
        [MaxLength(100)]
        public string DiscountCode { get; set; }

        [DataMember(Name = "Description")]
        [MaxLength(1024)]
        public string Description { get; set; }

        [DataMember(Name = "Discount")]
        public float Discount { get; set; }

        [DataMember(Name = "IsByRate")]
        public bool IsByRate { get; set; } = true;

        [DataMember(Name = "IsActive")]
        public bool IsActive { get; set; } = false;

        [DataMember(Name = "IsDeleted")]
        public bool IsDeleted { get; set; } = false;

        [DataMember(Name = "CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [DataMember(Name = "ModifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [DataMember(Name = "Createdby")]
        //[ForeignKey("CreatedUser")]
        public long Createdby { get; set; }
        //public UserDtOs.Users CreatedUser { get; set; }

        [DataMember(Name = "ModifiedBy")]
        //[ForeignKey("ModifierUser")]
        public long ModifiedBy { get; set; }
        //public UserDtOs.Users ModifierUser { get; set; }
    }
}

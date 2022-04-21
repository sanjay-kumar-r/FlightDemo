using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace AirlinesDTOs
{
    [DataContract(Name = "Airlines")]
    public class Airlines
    {
        [DataMember(Name = "Id")]
        //[key]
        public long Id { get; set; }

        [DataMember(Name = "Name")]
        [MaxLength(512)]
        public string Name { get; set; }

        [DataMember(Name = "AirlineCode")]
        [MaxLength(100)]
        public string AirlineCode { get; set; }

        [DataMember(Name = "ContactNumber")]
        [MaxLength(50)]
        public string ContactNumber { get; set; }

        [DataMember(Name = "ContactAddress")]
        [MaxLength(1024)]
        public string ContactAddress { get; set; }

        [DataMember(Name = "TotalSeats")]
        public int TotalSeats { get; set; }

        [DataMember(Name = "TotalBCSeats")]
        public int TotalBCSeats { get; set; }

        [DataMember(Name = "TotalNBCSeats")]
        public int TotalNBCSeats { get; set; }

        [DataMember(Name = "BCTicketCost")]
        public double BCTicketCost { get; set; }

        [DataMember(Name = "NBCTicketCost")]
        public double NBCTicketCost { get; set; }

        [DataMember(Name = "IsActive")]
        public bool IsActive { get; set; } = true;

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
       //[ForeignKey("ModifiedUser")]
        public long ModifiedBy { get; set; }
        //public UserDtOs.Users  ModifiedUser { get; set; }
    }

    [DataContract(Name = "AirlineDetails")]
    public class AirlineDetails
    {
        [DataMember(Name = "Id")]
        public long Id { get; set; }

        [DataMember(Name = "Name")]
        [MaxLength(512)]
        public string Name { get; set; }

        [DataMember(Name = "AirlineCode")]
        [MaxLength(100)]
        public string AirlineCode { get; set; }

        [DataMember(Name = "ContactNumber")]
        [MaxLength(50)]
        public string ContactNumber { get; set; }

        [DataMember(Name = "ContactAddress")]
        [MaxLength(1024)]
        public string ContactAddress { get; set; }

        [DataMember(Name = "TotalSeats")]
        public int TotalSeats { get; set; }

        [DataMember(Name = "TotalBCSeats")]
        public int TotalBCSeats { get; set; }

        [DataMember(Name = "TotalNBCSeats")]
        public int TotalNBCSeats { get; set; }

        [DataMember(Name = "BCTicketCost")]
        public double BCTicketCost { get; set; }

        [DataMember(Name = "NBCTicketCost")]
        public double NBCTicketCost { get; set; }

        [DataMember(Name = "IsActive")]
        public bool? IsActive { get; set; } = null;
    }
}

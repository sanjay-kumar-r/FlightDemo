using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace AirlinesDTOs
{
    [DataContract(Name = "DiscountTags")]
    public class AirlineDiscountTagMappings
    {

        [DataMember(Name = "Id")]
        public long Id { get; set; }

        [DataMember(Name = "AirlineId")]
        [ForeignKey("Airline")]
        public long AirlineId { get; set; }
        public AirlinesDTOs.Airlines Airline { get; set; }

        [DataMember(Name = "DiscountTagId")]
        [ForeignKey("DiscountTag")]
        public long DiscountTagId { get; set; }
        public AirlinesDTOs.DiscountTags DiscountTag { get; set; }

        [DataMember(Name = "TaggedBy")]
        //[ForeignKey("TaggedUser")]
        public long TaggedBy { get; set; }
        //public UserDtOs.Users TaggedUser { get; set; }
    }
}

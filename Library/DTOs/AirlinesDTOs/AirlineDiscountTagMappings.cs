using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace AirlinesDTOs
{
    [DataContract(Name = "AirlineDiscountTagMappings")]
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

    [DataContract(Name = "AirlineDiscountTagMappingDetails")]
    public class AirlineDiscountTagMappingDetails
    {
        [DataMember(Name = "Airline")]
        public AirlinesDTOs.AirlineDetails Airline { get; set; }

        [DataMember(Name = "DiscountTags")]
        public List<AirlinesDTOs.DiscountTagDetails> DiscountTags { get; set; }
    }

    [DataContract(Name = "RemapAirlineDiscountTagsDetails")]
    public class RemapAirlineDiscountTagsDetails
    {
        [DataMember(Name = "AirlineId")]
        public long AirlineId { get; set; }

        [DataMember(Name = "AddedDiscountTagIds")]
        public List<long> AddedDiscountTagIds { get; set; }

        [DataMember(Name = "RemovedDiscountTagIds")]
        public List<long> RemovedDiscountTagIds { get; set; }
    }
}

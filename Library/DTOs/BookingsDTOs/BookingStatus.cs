using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace BookingsDTOs
{
    [DataContract(Name = "BookingStatus")]
    public class BookingStatus
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

    public enum BookingStatusCode
    {
        Booked = 1,
        Waiting,
        Canceled,
        Refunded
    }

    public static class BookingStatusDescription
    {
        private static readonly Dictionary<string, string> _accountStatusDescriptions;
        public static Dictionary<string, string> accountStatusDescriptions
        {
            get
            {
                return _accountStatusDescriptions;
            }
        }
        static BookingStatusDescription()
        {
            _accountStatusDescriptions = new Dictionary<string, string>()
            {
                { "BOOKED", "When airline schedule successfully booked" },
                { "WAITING", "When seats are already filled" },
                { "CANCELED", "When user cancels Booking" },
                { "REFUNDED", "When user gets refunded back" }
            };
        }
    }
}

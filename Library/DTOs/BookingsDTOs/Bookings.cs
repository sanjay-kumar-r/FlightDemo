﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace BookingsDTOs
{
    [DataContract(Name = "Bookings")]
    public class Bookings
    {
        [DataMember(Name = "Id")]
        //[key]
        public long Id { get; set; }

        [DataMember(Name = "UserId")]
        //[ForeignKey("User")]
        public long UserId { get; set; }
        //public User User { get; set; }

        [DataMember(Name = "ScheduleId")]
        public long ScheduleId { get; set; }

        [DataMember(Name = "DateBookedFor")]
        public DateTime DateBookedFor { get; set; }

        [DataMember(Name = "BCSeats")]
        public long BCSeats { get; set; }

        [DataMember(Name = "NBCSeats")]
        public long NBCSeats { get; set; }

        [DataMember(Name = "ActualPaidAmount")]
        public double ActualPaidAmount { get; set; }

        [DataMember(Name = "BookingStatusId")]
        [ForeignKey("BookingStatus")]
        public int BookingStatusId { get; set; }
        public BookingStatus BookingStatus { get; set; }

        [DataMember(Name = "CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [DataMember(Name = "CanceledOn")]
        public DateTime? CanceledOn { get; set; }

        [DataMember(Name = "IsRefunded")]
        public bool? IsRefunded { get; set; } = null;
    }
}


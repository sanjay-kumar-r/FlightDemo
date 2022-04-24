using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace AirlinesDTOs
{
    [DataContract(Name = "AirlineSchedules")]
    public class AirlineSchedules
    {
        [DataMember(Name = "Id")]
        //[key]
        public long Id { get; set; }

        [DataMember(Name = "AirlineId")]
        [ForeignKey("Airline")]
        public long AirlineId { get; set; }
        public Airlines Airline { get; set; }

        [DataMember(Name = "From")]
        [MaxLength(512)]
        public string From { get; set; }

        [DataMember(Name = "To")]
        [MaxLength(512)]
        public string To { get; set; }

        [DataMember(Name = "IsRegular")]
        public bool IsRegular { get; set; } = true;

        [DataMember(Name = "DepartureDay")]
        public DayOfWeek? DepartureDay { get; set; }

        [DataMember(Name = "DepartureDate")]
        public DateTime? DepartureDate { get; set; }

        [DataMember(Name = "DepartureTime")]
        public DateTime DepartureTime { get; set; }

        [DataMember(Name = "ArrivalDay")]
        public DayOfWeek? ArrivalDay { get; set; }

        [DataMember(Name = "ArrivalDate")]
        public DateTime? ArrivalDate { get; set; }

        [DataMember(Name = "ArrivalTime")]
        public DateTime ArrivalTime { get; set; }

        [DataMember(Name = "CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [DataMember(Name = "ModifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        [DataMember(Name = "Createdby")]
        //[ForeignKey("CreatedUser")]
        public long Createdby { get; set; }
        //public UserDtOs.Users CreatedUser { get; set; }

        [DataMember(Name = "ModifiedBy")]
        //[ForeignKey("ModifiedUser")]
        public long ModifiedBy { get; set; }
        //public UserDtOs.Users  ModifiedUser { get; set; }

        [DataMember(Name = "IsDeleted")]
        public bool IsDeleted { get; set; } = false;
    }

    [DataContract(Name = "AirlineScheduleDetails")]
    public class AirlineScheduleDetails
    {
        [DataMember(Name = "Id")]
        public long Id { get; set; }

        [DataMember(Name = "AirlineId")]
        public long AirlineId { get; set; }

        [DataMember(Name = "From")]
        [MaxLength(512)]
        public string From { get; set; }

        [DataMember(Name = "To")]
        [MaxLength(512)]
        public string To { get; set; }

        [DataMember(Name = "IsRegular")]
        public bool? IsRegular { get; set; }

        [DataMember(Name = "DepartureDay")]
        public DayOfWeek? DepartureDay { get; set; }

        [DataMember(Name = "DepartureDate")]
        public DateTime? DepartureDate { get; set; }

        [DataMember(Name = "DepartureTime")]
        public DateTime? DepartureTime { get; set; }

        [DataMember(Name = "ArrivalDay")]
        public DayOfWeek? ArrivalDay { get; set; }

        [DataMember(Name = "ArrivalDate")]
        public DateTime? ArrivalDate { get; set; }

        [DataMember(Name = "ArrivalTime")]
        public DateTime? ArrivalTime { get; set; }
    }

    [DataContract(Name = "AirlineScheduleTracker")]
    public class AirlineScheduleTracker
    {
        [DataMember(Name = "Id")]
        public long Id { get; set; }

        [DataMember(Name = "ScheduleId")]
        [ForeignKey("AirlineSchedule")]
        public long ScheduleId { get; set; }
        public AirlineSchedules AirlineSchedule { get; set; }

        [DataMember(Name = "ActualDepartureDate")]
        public DateTime ActualDepartureDate { get; set; }

        //[DataMember(Name = "ActualDepartureTime")]
        //public DateTime ActualDepartureTime { get; set; }

        [DataMember(Name = "ActualArrivalDate")]
        public DateTime? ActualArrivalDate { get; set; }

        //[DataMember(Name = "ActualArrivalTime")]
        //public DateTime ActualArrivalTime { get; set; }

        [DataMember(Name = "BCSeatsRemaining")]
        public int BCSeatsRemaining { get; set; }

        [DataMember(Name = "NBCSeatsRemaining")]
        public int NBCSeatsRemaining { get; set; }

        [DataMember(Name = "IsDeleted")]
        public bool IsDeleted { get; set; } = false;
    }

    [DataContract(Name = "AirlineScheduleTrackerDetails")]
    public class AirlineScheduleTrackerDetails
    {
        [DataMember(Name = "Id")]
        public long Id { get; set; }

        [DataMember(Name = "ScheduleId")]
        public long ScheduleId { get; set; }

        [DataMember(Name = "ActualDepartureDate")]
        public DateTime? ActualDepartureDate { get; set; }

        [DataMember(Name = "ActualArrivalDate")]
        public DateTime? ActualArrivalDate { get; set; }

        [DataMember(Name = "BCSeatsRemaining")]
        public int BCSeatsRemaining { get; set; }

        [DataMember(Name = "NBCSeatsRemaining")]
        public int NBCSeatsRemaining { get; set; }
    }
}

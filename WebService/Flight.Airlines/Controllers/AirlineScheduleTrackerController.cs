using AirlinesDTOs;
using BookingsDTOs;
using CommonDTOs;
using Flight.Airlines.Models.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceContracts.Airlines;
using ServiceContracts.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Flight.Airlines.Controllers
{
    //[Authorize(Roles = "admin")]
    [Authorize]
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class AirlineScheduleTrackerController : ControllerBase
    {
        private readonly CustomSettings customSettings;
        //private readonly IConfiguration config;
        private readonly IAirlinesRepository airlinesRepo;
        private readonly ILogger logger;

        public AirlineScheduleTrackerController(IConfiguration config, IAirlinesRepository airlinesRepo, ILogger logger)
        {
            customSettings = new CustomSettings();
            config.GetSection("CustomSettings").Bind(customSettings);
            //this.config = config;
            this.airlinesRepo = airlinesRepo;
            this.logger = logger;
        }

        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "AirlineScheduleTrackerController -> Pong";
        }

        //Search available flights
        [HttpPost]
        [Route("GetAvailableAirlines")]
        public IEnumerable<AirlinesSearchResponse> GetAvailableAirlines([FromBody] AirlinesSearchRequest airlinesSearchRequest)
        {
            if (airlinesSearchRequest.DepartureDate == null)
                airlinesSearchRequest.DepartureDate = DateTime.Now.Date;
            //if (airlinesSearchRequest.ArrivalDate == null)
            //    airlinesSearchRequest.ArrivalDate = DateTime.Now.AddDays(7).Date;
            if (!AirlinesValidation.ValidateGetAvailableAirlines(airlinesSearchRequest))
            {
                logger.Log(LogLevel.ERROR, "AirlinesValidation.ValidateGetAvailableAirlines Failed");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("AirlinesValidation.ValidateGetAvailableAirlines Failed");
            }

            var airlinesSearchResponse = new List<AirlinesSearchResponse>();
            //get available airlineSchedules
            var scheduleSearch = new AirlineScheduleDetails()
            {
                From = airlinesSearchRequest.From,
                To = airlinesSearchRequest.To,
                DepartureDay = airlinesSearchRequest.DepartureDate.Value.DayOfWeek,
                DepartureDate = airlinesSearchRequest.DepartureDate.Value.Date
            };
            var d1 = airlinesSearchRequest.DepartureDate.Value.Date;
            var d2 = DateTime.Now.AddDays(3);
            if(d1.Date > d2.Date)
            { bool x = true; }
            var availableSchedules = airlinesRepo.GetGetAirlineSchedulesByFilterCondition(scheduleSearch);
            if (availableSchedules != null && availableSchedules.Count() > 0)
            {
                //get active airlineSchedules
                var activeSchedules = availableSchedules.Where(x => x.Airline != null && !x.Airline.IsDeleted && x.Airline.IsActive);
                if (activeSchedules != null && activeSchedules.Count() > 0)
                {
                    foreach (var schedule in activeSchedules)
                    {
                        var response = new AirlinesSearchResponse()
                        {
                            AirlineSchedules = schedule,
                            BCSeatsAvailable = schedule.Airline.TotalBCSeats,
                            NBCSeatsAvailable = schedule.Airline.TotalNBCSeats,
                        };
                        if (!schedule.IsRegular)
                        {
                            response.ActualDepartureDate = schedule.DepartureDate.Value.Date;
                            response.ActualArrivalDate = schedule.ArrivalDate.Value.Date;
                        }
                        else
                        {
                            var today = DateTime.Now.DayOfWeek;
                            if ((int)schedule.DepartureDay.Value >= (int)today)
                                response.ActualDepartureDate = DateTime.Now.AddDays((int)schedule.DepartureDay.Value - (int)today);
                            else
                            {
                                response.ActualDepartureDate = DateTime.Now.AddDays(
                                    ((int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)today)
                                    + (int)schedule.DepartureDay.Value + 1);
                            }
                            if ((int)schedule.ArrivalDay.Value >= (int)today)
                                response.ActualArrivalDate = DateTime.Now.AddDays((int)schedule.ArrivalDay.Value - (int)today);
                            else
                            {
                                response.ActualArrivalDate = DateTime.Now.AddDays(
                                    ((int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)today)
                                    + (int)schedule.ArrivalDay.Value + 1);
                            }
                        }
                        response.ActualDepartureDate = response.ActualDepartureDate +
                            new TimeSpan(schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
                        response.ActualArrivalDate = response.ActualArrivalDate +
                            new TimeSpan(schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
                        //get active discountTags and add to result
                        var discountTagMappings = airlinesRepo.GetAirlineDiscountTagsMappings(schedule.AirlineId);
                        if (discountTagMappings != null && discountTagMappings.Count() > 0)
                        {
                            var activeDiscountTags = discountTagMappings.Where(x => !x.DiscountTag.IsDeleted && x.DiscountTag.IsActive);
                            if (activeDiscountTags != null && activeDiscountTags.Count() > 0)
                            {
                                response.DiscountTags = activeDiscountTags.Select(x => x.DiscountTag).ToList();
                            }
                        }
                        //get available seats
                        var scheduleTrackerSearch = new AirlineScheduleTracker()
                        {
                            ScheduleId = schedule.Id,
                            //ActualDepartureDate = airlinesSearchRequest.DepartureDate.Value.Date
                            ActualDepartureDate = response.ActualDepartureDate.Value
                        };
                        var scheduleTrackers = airlinesRepo.GetAirlineScheduleTrackerByFilterCondition(scheduleTrackerSearch);
                        if (scheduleTrackers != null && scheduleTrackers.Count() > 0 && scheduleTrackers.FirstOrDefault() != null)
                        {
                            response.BCSeatsAvailable = scheduleTrackers.FirstOrDefault().BCSeatsRemaining;
                            response.NBCSeatsAvailable = scheduleTrackers.FirstOrDefault().NBCSeatsRemaining;
                        }
                        //add the prepared response to final list
                        airlinesSearchResponse.Add(response);
                    }
                }
            }
            return airlinesSearchResponse;
        }
        [HttpPost]
        [Route("CheckForAvailableSeatsAndAddTracker")]
        public BookingStatusCode CheckForAvailableSeatsAndAddTracker([FromBody] AirlineScheduleTracker airlineScheduleTracker)
        {
            if (AirlinesValidation.ValidateCheckForAvailableSeats(airlineScheduleTracker))
            {
                logger.Log(LogLevel.ERROR, "AirlinesValidation.ValidateCheckForAvailableSeats Failed");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("AirlinesValidation.ValidateCheckForAvailableSeats Failed");
            }

            if (airlineScheduleTracker.BCSeatsRemaining < 0)
                airlineScheduleTracker.BCSeatsRemaining = 0;
            if (airlineScheduleTracker.NBCSeatsRemaining < 0)
                airlineScheduleTracker.NBCSeatsRemaining = 0;

            BookingStatusCode response = BookingStatusCode.Invalid;
            //validate if scheduleId exist
            var schedules = airlinesRepo.GetAirlineSchedules(airlineScheduleTracker.ScheduleId);
            if (schedules != null && schedules.Count() > 0 && schedules.FirstOrDefault() != null)
            {
                //Validate proper schedule time
                var schedule = schedules.FirstOrDefault(x => 
                (x.IsRegular && x.DepartureDay == airlineScheduleTracker.ActualDepartureDate.DayOfWeek
                    && x.DepartureTime.ToShortTimeString().Equals(airlineScheduleTracker.ActualDepartureDate.ToShortTimeString()))
                || (!x.IsRegular && x.DepartureDate?.Date == airlineScheduleTracker.ActualDepartureDate.Date
                    && x.DepartureTime.ToShortTimeString().Equals(airlineScheduleTracker.ActualDepartureDate.ToShortTimeString())));

                //validate if airline is active
                if (schedule != null && schedule.Id > 0 && !schedule.Airline.IsDeleted && schedule.Airline.IsActive)
                {
                    //prepare search request - just to be sure that not passing and filtering by any other parameters
                    var scheduleTrackerSearch = new AirlineScheduleTracker()
                    {
                        ScheduleId = schedule.Id,
                        ActualDepartureDate = airlineScheduleTracker.ActualDepartureDate
                    };
                    //check if tracker already exists
                    var scheduleTrackers = airlinesRepo.GetAirlineScheduleTrackerByFilterCondition(scheduleTrackerSearch);
                    if (scheduleTrackers != null && scheduleTrackers.Count() > 0 && scheduleTrackers.FirstOrDefault() != null)
                    {
                        var tracker = scheduleTrackers.FirstOrDefault();
                        //check seats are available
                        if ((airlineScheduleTracker.BCSeatsRemaining <= 0 ||
                                tracker.BCSeatsRemaining >= airlineScheduleTracker.BCSeatsRemaining)
                            && (airlineScheduleTracker.NBCSeatsRemaining <= 0 ||
                                tracker.NBCSeatsRemaining >= airlineScheduleTracker.NBCSeatsRemaining))
                        {
                            bool result = airlinesRepo.UpdateAirlineScheduleTracker(tracker.Id,
                                airlineScheduleTracker.BCSeatsRemaining, airlineScheduleTracker.NBCSeatsRemaining);
                            if (result)
                            {
                                response = BookingStatusCode.Booked;
                            }
                        }
                        else
                        {
                            //seats are not available
                            response = BookingStatusCode.Waiting;
                        }
                    }
                    else
                    {
                        //add new tracker
                        if ((airlineScheduleTracker.BCSeatsRemaining <= 0 ||
                                schedule.Airline.TotalBCSeats >= airlineScheduleTracker.BCSeatsRemaining)
                            && (airlineScheduleTracker.NBCSeatsRemaining <= 0 ||
                                schedule.Airline.TotalNBCSeats >= airlineScheduleTracker.NBCSeatsRemaining))
                        {
                            var tracker = new AirlineScheduleTracker()
                            {
                                ScheduleId = schedule.Id,
                                BCSeatsRemaining = schedule.Airline.TotalBCSeats - airlineScheduleTracker.BCSeatsRemaining,
                                NBCSeatsRemaining = schedule.Airline.TotalNBCSeats - airlineScheduleTracker.NBCSeatsRemaining
                            };
                            if (!schedule.IsRegular)
                            {
                                tracker.ActualDepartureDate = schedule.DepartureDate.Value.Date;
                                tracker.ActualArrivalDate = schedule.ArrivalDate.Value.Date;
                            }
                            else
                            {
                                var today = DateTime.Now.DayOfWeek;
                                if ((int)schedule.DepartureDay.Value >= (int)today)
                                    tracker.ActualDepartureDate = DateTime.Now.AddDays((int)schedule.DepartureDay.Value - (int)today);
                                else
                                {
                                    tracker.ActualDepartureDate = DateTime.Now.AddDays(
                                        ((int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)today)
                                        + (int)schedule.DepartureDay.Value + 1);
                                }
                                if ((int)schedule.ArrivalDay.Value >= (int)today)
                                    tracker.ActualArrivalDate = DateTime.Now.AddDays((int)schedule.ArrivalDay.Value - (int)today);
                                else
                                {
                                    tracker.ActualArrivalDate = DateTime.Now.AddDays(
                                        ((int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)today)
                                        + (int)schedule.ArrivalDay.Value + 1);
                                }
                            }
                            tracker.ActualDepartureDate = tracker.ActualDepartureDate +
                                new TimeSpan(schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
                            tracker.ActualArrivalDate = tracker.ActualArrivalDate +
                                new TimeSpan(schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
                            tracker.IsDeleted = false;
                            long trackerId = airlinesRepo.AddAirlineScheduleTracker(tracker);
                            if (trackerId > 0)
                            {
                                //seats are available
                                response = BookingStatusCode.Booked;
                            }
                        }
                        else
                        {
                            //invalid seats booking
                        }
                    }
                }
            }
            return response;
        }
        [HttpPost]
        [Route("RevertScheduleTracker")]
        public bool RevertScheduleTracker([FromBody] AirlineScheduleTracker airlineScheduleTracker)
        {
            if (AirlinesValidation.RevertScheduleTracker(airlineScheduleTracker))
            {
                logger.Log(LogLevel.ERROR, "AirlinesValidation.RevertScheduleTracker Failed");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("AirlinesValidation.RevertScheduleTracker Failed");
            }

            if (airlineScheduleTracker.BCSeatsRemaining < 0)
                airlineScheduleTracker.BCSeatsRemaining = 0;
            if (airlineScheduleTracker.NBCSeatsRemaining < 0)
                airlineScheduleTracker.NBCSeatsRemaining = 0;

            bool result = true;
            //prepare search request - just to be sure that not passing and filtering by any other parameters
            var scheduleTrackerSearch = new AirlineScheduleTracker()
            {
                ScheduleId = airlineScheduleTracker.ScheduleId,
                ActualDepartureDate = airlineScheduleTracker.ActualDepartureDate
            };
            //get existing tracker
            var scheduleTrackers = airlinesRepo.GetAirlineScheduleTrackerByFilterCondition(scheduleTrackerSearch);
            if (scheduleTrackers != null && scheduleTrackers.Count() > 0 && scheduleTrackers.FirstOrDefault() != null)
            {
                var tracker = scheduleTrackers.FirstOrDefault();
                if (!tracker.AirlineSchedule.Airline.IsDeleted && tracker.AirlineSchedule.Airline.IsActive)
                {
                    if ((tracker.BCSeatsRemaining + airlineScheduleTracker.BCSeatsRemaining <= tracker.AirlineSchedule.Airline.TotalBCSeats)
                        && (tracker.NBCSeatsRemaining + airlineScheduleTracker.NBCSeatsRemaining <= tracker.AirlineSchedule.Airline.TotalNBCSeats))
                    {
                        result = airlinesRepo.UpdateAirlineScheduleTracker(tracker.Id,
                        airlineScheduleTracker.BCSeatsRemaining, airlineScheduleTracker.NBCSeatsRemaining, true);
                    }
                }
            }
            return result;
        }
    }
}

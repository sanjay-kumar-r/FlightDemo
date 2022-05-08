using AirlinesDTOs;
using BookingsDTOs;
using CommonDTOs;
using Flight.Airlines.Models.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ServiceContracts.Airlines;
using ServiceContracts.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Flight.Airlines.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class AirlinesController : ControllerBase
    {
        private readonly CustomSettings customSettings;
        //private readonly IConfiguration config;
        private readonly IAirlinesRepository airlinesRepo;
        private readonly ILogger logger;

        public AirlinesController(IConfiguration config, IAirlinesRepository airlinesRepo, ILogger logger)
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
            return "AirlinesController -> Pong";
        }

        [HttpGet]
        public IEnumerable<AirlinesDTOs.Airlines> Get()
        {
            return airlinesRepo.GetAirlines();
        }

        [HttpGet]
        [Route("{id}")]
        //[Route("GetAirlines/{id}")]
        public IEnumerable<AirlinesDTOs.Airlines> Get(long id)
        {
            return airlinesRepo.GetAirlines(id);
        }

        [HttpPost]
        [Route("GetAirlinesByFiltercondition")]
        public IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition([FromBody] AirlinesDTOs.AirlineDetails airline)
        {
            return airlinesRepo.GetAirlinesByFiltercondition(airline);
        }

        [HttpPost]
        [Route("Add")]
        public long Add([FromBody] AirlinesDTOs.Airlines airline)
        {
            if (!AirlinesValidation.ValidateAddFlight(airline))
            {
                logger.Log(LogLevel.ERROR, "AirlinesValidation.ValidateAddFlight Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("AirlinesValidation.ValidateAddFlight Falied");
            }

            if (airlinesRepo.IsAirlineAlreadyExists(airline))
            {
                logger.Log(LogLevel.ERROR, "Airline name and/or code already exists");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Duplicate, CustomErrorMessage = "Airline name and/or code already exists" };
                //throw new Exception("Airline name and/or code already exists");
            }

            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            airline.Createdby = Convert.ToInt64(userId);
            airline.ModifiedBy = Convert.ToInt64(userId);
            return airlinesRepo.AddAirline(airline);
        }

        [HttpPost]
        [Route("Update")]
        public Result Update([FromBody] AirlinesDTOs.AirlineDetails airline)
        {
            if (!AirlinesValidation.ValidateUpdateFlight(airline))
            {
                logger.Log(LogLevel.ERROR, "AirlinesValidation.ValidateUpdateFlight Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("AirlinesValidation.ValidateUpdateFlight Falied");
            }

            AirlinesDTOs.Airlines airline_1 = new AirlinesDTOs.Airlines()
            {
                Id = airline.Id,
                Name = airline.Name,
                AirlineCode = airline.AirlineCode,
                ContactNumber = airline.ContactNumber,
                ContactAddress = airline.ContactAddress,
                TotalSeats = airline.TotalSeats,
                TotalBCSeats = airline.TotalBCSeats,
                TotalNBCSeats = airline.TotalNBCSeats,
                BCTicketCost = airline.BCTicketCost,
                NBCTicketCost = airline.NBCTicketCost
            };
            if (airline.IsActive != null)
                airline_1.IsActive = (bool)airline.IsActive;
            if (airlinesRepo.IsAirlineAlreadyExists(airline_1))
            {
                logger.Log(LogLevel.ERROR, "Airline name and/or code already exists");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Duplicate, CustomErrorMessage = "Airline name and/or code already exists" };
                //throw new Exception("Airline name and/or code already exists");
            }

            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            return airlinesRepo.UpdateAirline(airline, userId);
        }

        [HttpPost]
        [Route("ActivateDeactivateAirline")]
        public Result ActivateDeactivateAirline([FromBody] dynamic obj)
        {
            //long id = Convert.ToInt64(airline["Id"].ToString());
            if (!AirlinesValidation.ValidateActivateDeactivateAirline(obj.GetProperty("Id"), obj.GetProperty("IsActive")))
            {
                logger.Log(LogLevel.ERROR, "AirlinesValidation.ValidateActivateDeactivateAirline Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("AirlinesValidation.ValidateActivateDeactivateAirline Falied");
            }

            AirlinesDTOs.Airlines airline = new AirlinesDTOs.Airlines()
            {
                Id = Convert.ToInt64(obj.GetProperty("Id").ToString()),
                IsActive = Convert.ToBoolean(obj.GetProperty("IsActive").ToString().Trim()),
                ModifiedBy = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"])
            };
            return airlinesRepo.ActivateDeactivateAirline(airline);
        }

        [HttpPost]
        [Route("Delete")]
        public Result Delete([FromBody] long id)
        {
            var airline = new AirlinesDTOs.Airlines()
            {
                Id = id,
                ModifiedBy = Convert.ToInt64(HttpContext.Request.Headers["UserId"])
            };
            return airlinesRepo.DeleteAirline(airline);
        }

        [HttpPost]
        [Route("PermanentDelete")]
        public Result PermanentDelete([FromBody] long id)
        {
            return airlinesRepo.PermanentDeleteAirline(id);
        }

        [HttpPost]
        [Route("MapAirlinesDiscountTags")]
        public bool MapAirlinesDiscountTags([FromBody] List<AirlinesDTOs.AirlineDiscountTagMappingDetails> airlineDiscountTagMappingDetails)
        {
            if (!AirlinesValidation.ValidateAirlineDiscountTagMappings(airlineDiscountTagMappingDetails))
            {
                logger.Log(LogLevel.ERROR, "AirlinesValidation.ValidateAirlineDiscountTagMappings Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("AirlinesValidation.ValidateAirlineDiscountTagMappings Falied");
            }

            List<AirlineDiscountTagMappings> airlineDiscountTagMappings = new List<AirlineDiscountTagMappings>();
            var mappings = airlineDiscountTagMappingDetails.Where(x => x.Airline != null
                && (x.Airline.Id > 0 || !string.IsNullOrWhiteSpace(x.Airline.Name) || !string.IsNullOrWhiteSpace(x.Airline.AirlineCode))
                && x.DiscountTags != null);
            if (mappings != null && mappings.Count() > 0)
            {
                long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
                foreach (var map in mappings)
                {
                    var airline = airlinesRepo.GetAirlinesByFiltercondition(map.Airline);
                    if (airline != null && airline.Count() > 0)
                    {
                        long airlineId = airline.FirstOrDefault().Id;
                        var discountTags = map.DiscountTags.Where(x => x.Id > 0
                                || !string.IsNullOrWhiteSpace(x.Name) || !string.IsNullOrWhiteSpace(x.DiscountCode));
                        if (discountTags != null && discountTags.Count() > 0)
                        {
                            var discountTagDetails = airlinesRepo.GetDiscountTagsByMultipleFilterconditions(discountTags.ToList());
                            if (discountTagDetails != null && discountTagDetails.Count() > 0)
                            {
                                airlineDiscountTagMappings.AddRange(discountTagDetails.
                                    Select(x => new AirlineDiscountTagMappings()
                                    {
                                        AirlineId = airlineId,
                                        DiscountTagId = x.Id,
                                        TaggedBy = userId
                                        ,Airline = null,
                                        DiscountTag = null
                                    }).ToList());
                            }
                        }
                    }
                }

                if (airlineDiscountTagMappings == null || airlineDiscountTagMappings.Count() <= 0)
                {
                    logger.Log(LogLevel.ERROR, "All input mappings are invalid");
                    throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "invalid input" };
                    //throw new Exception("All input mappings are invalid");
                }

                return airlinesRepo.AddAirlineDiscountTagMappings(airlineDiscountTagMappings);
            }
            else
                return false;
            //{
            //    return new Result() { Res = false, ResultMessage = "input airlineDiscountTagMappingDetails are invalid" };
            //}
        }

        [HttpPost]
        [Route("RemapAirlinesDiscountTags")]
        public bool RemapAirlinesDiscountTags([FromBody] List<AirlinesDTOs.RemapAirlineDiscountTagsDetails> remapAirlineDiscountTagsDetails)
        {
            bool result = false;
            if (!AirlinesValidation.ValidateRemapAirlineDiscountTagsDetails(remapAirlineDiscountTagsDetails))
            {
                logger.Log(LogLevel.ERROR, "AirlinesValidation.ValidateRemapAirlineDiscountTagsDetails Falied");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "validation failed" };
                //throw new Exception("AirlinesValidation.ValidateRemapAirlineDiscountTagsDetails Falied");
            }

            List<AirlineDiscountTagMappings> addedAirlineDiscountTagMappings = new List<AirlineDiscountTagMappings>();
            List<AirlineDiscountTagMappings> removedAirlineDiscountTagMappings = new List<AirlineDiscountTagMappings>();
            var mappings = remapAirlineDiscountTagsDetails.Where(x => x.AirlineId > 0
                && (x.AddedDiscountTagIds != null || x.RemovedDiscountTagIds != null));
            if (mappings != null && mappings.Count() > 0)
            {
                long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
                foreach (var map in mappings)
                {
                    var airline = airlinesRepo.GetAirlines(map.AirlineId);
                    if (airline != null && airline.Count() > 0 && airline.FirstOrDefault() != null)
                    {
                        long airlineId = airline.FirstOrDefault().Id;
                        if (map.AddedDiscountTagIds != null && map.AddedDiscountTagIds.Count() > 0)
                        {
                            var addedDiscountTagIds = map.AddedDiscountTagIds.Except(map.RemovedDiscountTagIds ?? new List<long>());
                            if (addedDiscountTagIds != null && addedDiscountTagIds.Count() > 0)
                            {
                                var discountTagDetails = airlinesRepo.GetDiscountTagByIds(addedDiscountTagIds.ToList());
                                if (discountTagDetails != null && discountTagDetails.Count() > 0)
                                {
                                    addedAirlineDiscountTagMappings.AddRange(discountTagDetails.
                                        Select(x => new AirlineDiscountTagMappings()
                                        {
                                            AirlineId = airlineId,
                                            DiscountTagId = x.Id,
                                            TaggedBy = userId,
                                            Airline = null,
                                            DiscountTag = null
                                        }));
                                }
                            }
                        }
                        if (map.RemovedDiscountTagIds != null && map.RemovedDiscountTagIds.Count() > 0)
                        {
                            removedAirlineDiscountTagMappings.AddRange(map.RemovedDiscountTagIds.
                                Select(x => new AirlineDiscountTagMappings()
                                {
                                    AirlineId = airline.FirstOrDefault().Id,
                                    DiscountTagId = x,
                                    Airline = null,
                                    DiscountTag = null
                                    //TaggedBy = userId,
                                    //Airline = null,
                                    //DiscountTag = null
                                }));
                        }
                    }
                }

                if ((addedAirlineDiscountTagMappings == null || addedAirlineDiscountTagMappings.Count() <= 0)
                    && (removedAirlineDiscountTagMappings == null || removedAirlineDiscountTagMappings.Count() <= 0))
                {
                    logger.Log(LogLevel.ERROR, "All input mappings are invalid");
                    throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, CustomErrorMessage = "All input mappings are invalid" };
                    //throw new Exception("All input mappings are invalid");
                }

                if (addedAirlineDiscountTagMappings != null && addedAirlineDiscountTagMappings.Count() > 0)
                    result = airlinesRepo.AddAirlineDiscountTagMappings(addedAirlineDiscountTagMappings);
                if (removedAirlineDiscountTagMappings != null && removedAirlineDiscountTagMappings.Count() > 0)
                    result = airlinesRepo.RemoveAirlineDiscountTagMappings(removedAirlineDiscountTagMappings);
                return result;
            }
            else
                return result;
            //return new Result() { Res = false, ResultMessage = "input remapAirlineDiscountTagsDetails are invalid" };
        }

        [HttpGet]
        [Route("GetAirlineDiscountTagsMapping")]
        public IEnumerable<AirlineDiscountTagMappingDetails> GetAirlineDiscountTagsMapping()
        {
            IEnumerable<AirlineDiscountTagMappingDetails> airlineDiscountTagsMappingDetails = null;
            var airlineDiscountTagsMappings = airlinesRepo.GetAirlineDiscountTagsMappings();
            if (airlineDiscountTagsMappings != null && airlineDiscountTagsMappings.Count() > 0)
            {
                airlineDiscountTagsMappingDetails = airlineDiscountTagsMappings.GroupBy(x => x.AirlineId).Select(x =>
                new AirlineDiscountTagMappingDetails()
                {
                    Airline = new AirlineDetails()
                    {
                        Id = x.Key,
                        Name = x.FirstOrDefault().Airline.Name,
                        AirlineCode = x.FirstOrDefault().Airline.AirlineCode,
                        ContactNumber = x.FirstOrDefault().Airline.ContactNumber,
                        ContactAddress = x.FirstOrDefault().Airline.ContactAddress,
                        TotalSeats = x.FirstOrDefault().Airline.TotalSeats,
                        TotalBCSeats = x.FirstOrDefault().Airline.TotalBCSeats,
                        TotalNBCSeats = x.FirstOrDefault().Airline.TotalNBCSeats,
                        BCTicketCost = x.FirstOrDefault().Airline.BCTicketCost,
                        NBCTicketCost = x.FirstOrDefault().Airline.NBCTicketCost,
                        IsActive = x.FirstOrDefault().Airline.IsActive
                    },
                    DiscountTags = x.Select(y => new DiscountTagDetails()
                    {
                        Id = y.DiscountTag.Id,
                        Name = y.DiscountTag.Name,
                        DiscountCode = y.DiscountTag.DiscountCode,
                        Description = y.DiscountTag.Description,
                        Discount = y.DiscountTag.Discount,
                        IsByRate = y.DiscountTag.IsByRate,
                        IsActive = y.DiscountTag.IsActive
                    }).ToList()
                });
            }
            return airlineDiscountTagsMappingDetails;
        }

        [HttpGet]
        [Route("GetAirlineDiscountTagsMapping/{airlineId}")]
        [Route("GetAirlineDiscountTagsMapping/{airlineId}/{discountId}")]
        public IEnumerable<AirlineDiscountTagMappingDetails> GetAirlineDiscountTagsMapping(long airlineId = 0, long discountId = 0)
        {
            if (airlineId <= 0 && discountId <= 0)
            {
                logger.Log(LogLevel.ERROR, "Atleast one of the fields (airlineId or discountId) should be be greated than zero");
                throw new CustomException() { CustomErrorCode = CustomErrorCode.Invalid, 
                    CustomErrorMessage = "Atleast one of the fields (airlineId or discountId) should be be greated than zero" };
                //throw new Exception("Atleast one of the fields (airlineId or discountId) should be be greated than zero");
            }

            var airlineDiscountTagsMappingDetails = new List<AirlineDiscountTagMappingDetails>();
            var airlineDiscountTagsMappings = airlinesRepo.GetAirlineDiscountTagsMappings(airlineId, discountId);
            if (airlineDiscountTagsMappings != null && airlineDiscountTagsMappings.Count() > 0)
            {
                airlineDiscountTagsMappingDetails = airlineDiscountTagsMappings.GroupBy(x => x.AirlineId).Select(x =>
                new AirlineDiscountTagMappingDetails()
                {
                    Airline = new AirlineDetails()
                    {
                        Id = x.Key,
                        Name = x.FirstOrDefault().Airline.Name,
                        AirlineCode = x.FirstOrDefault().Airline.AirlineCode,
                        ContactNumber = x.FirstOrDefault().Airline.ContactNumber,
                        ContactAddress = x.FirstOrDefault().Airline.ContactAddress,
                        TotalSeats = x.FirstOrDefault().Airline.TotalSeats,
                        TotalBCSeats = x.FirstOrDefault().Airline.TotalBCSeats,
                        TotalNBCSeats = x.FirstOrDefault().Airline.TotalNBCSeats,
                        BCTicketCost = x.FirstOrDefault().Airline.BCTicketCost,
                        NBCTicketCost = x.FirstOrDefault().Airline.NBCTicketCost,
                        IsActive = x.FirstOrDefault().Airline.IsActive
                    },
                    DiscountTags = x.Select(y => new DiscountTagDetails()
                    {
                        Id = y.DiscountTag.Id,
                        Name = y.DiscountTag.Name,
                        DiscountCode = y.DiscountTag.DiscountCode,
                        Description = y.DiscountTag.Description,
                        Discount = y.DiscountTag.Discount,
                        IsByRate = y.DiscountTag.IsByRate,
                        IsActive = y.DiscountTag.IsActive
                    }).ToList()
                }).ToList();
            }
            return airlineDiscountTagsMappingDetails;
        }

        ////Search available flights
        //[HttpPost]
        //[Route("GetAvailableAirlines")]
        //public IEnumerable<AirlinesSearchResponse> GetAvailableAirlines([FromBody] AirlinesSearchRequest airlinesSearchRequest)
        //{
        //    if (airlinesSearchRequest.DepartureDate == null)
        //        airlinesSearchRequest.DepartureDate = DateTime.Now.Date;
        //    if (airlinesSearchRequest.ArrivalDate == null)
        //        airlinesSearchRequest.ArrivalDate = DateTime.Now.AddDays(7).Date;
        //    if (!AirlinesValidation.ValidateGetAvailableAirlines(airlinesSearchRequest))
        //        throw new Exception("AirlinesValidation.ValidateGetAvailableAirlines Failed");

        //    var airlinesSearchResponse = new List<AirlinesSearchResponse>();
        //    //get available airlineSchedules
        //    var scheduleSearch = new AirlineScheduleDetails()
        //    {
        //        From = airlinesSearchRequest.From,
        //        To = airlinesSearchRequest.To,
        //        DepartureDay = null,
        //        DepartureDate = airlinesSearchRequest.DepartureDate.Value.Date
        //    };
        //    var availableSchedules = airlineRepo.GetGetAirlineSchedulesByFilterCondition(scheduleSearch);
        //    if(availableSchedules != null && availableSchedules.Count() > 0)
        //    {
        //        //get active airlineSchedules
        //        var activeSchedules = availableSchedules.Where(x => x.Airline != null && !x.Airline.IsDeleted && x.Airline.IsActive);
        //        if(activeSchedules != null && activeSchedules.Count() > 0)
        //        {
        //            foreach (var schedule in activeSchedules)
        //            {
        //                var response = new AirlinesSearchResponse()
        //                {
        //                    AirlineSchedules = schedule,
        //                    BCSeatsAvailable = schedule.Airline.TotalBCSeats,
        //                    NBCSeatsAvailable = schedule.Airline.TotalNBCSeats,
        //                };
        //                if (!schedule.IsRegular)
        //                {
        //                    response.ActualDepartureDate = schedule.DepartureDate.Value.Date;
        //                    response.ActualArrivalDate = schedule.ArrivalDate.Value.Date;
        //                }
        //                else
        //                {
        //                    var today = DateTime.Now.DayOfWeek;
        //                    if((int)schedule.DepartureDay.Value >= (int)today)
        //                        response.ActualDepartureDate = DateTime.Now.AddDays((int)schedule.DepartureDay.Value - (int)today);
        //                    else
        //                    {
        //                        response.ActualDepartureDate = DateTime.Now.AddDays(
        //                            ((int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)today) 
        //                            + (int)schedule.DepartureDay.Value + 1);
        //                    }
        //                    if ((int)schedule.ArrivalDay.Value >= (int)today)
        //                        response.ActualArrivalDate = DateTime.Now.AddDays((int)schedule.ArrivalDay.Value - (int)today);
        //                    else
        //                    {
        //                        response.ActualArrivalDate = DateTime.Now.AddDays(
        //                            ((int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)today)
        //                            + (int)schedule.ArrivalDay.Value + 1);
        //                    }
        //                }
        //                response.ActualDepartureDate = response.ActualDepartureDate +
        //                    new TimeSpan(schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
        //                response.ActualArrivalDate = response.ActualArrivalDate +
        //                    new TimeSpan(schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
        //                //get active discountTags and add to result
        //                var discountTagMappings = airlineRepo.GetAirlineDiscountTagsMappings(schedule.AirlineId);
        //                if(discountTagMappings != null && discountTagMappings.Count() > 0)
        //                {
        //                    var activeDiscountTags = discountTagMappings.Where(x => !x.DiscountTag.IsDeleted && x.DiscountTag.IsActive);
        //                    if(activeDiscountTags != null && activeDiscountTags.Count() > 0)
        //                    {
        //                        response.DiscountTags = activeDiscountTags.Select(x => x.DiscountTag).ToList();
        //                    }
        //                }
        //                //get available seats
        //                var scheduleTrackerSearch = new AirlineScheduleTracker()
        //                {
        //                    ScheduleId = schedule.Id,
        //                    ActualDepartureDate = airlinesSearchRequest.DepartureDate.Value.Date
        //                };
        //                var scheduleTrackers = airlineRepo.GetAirlineScheduleTrackerByFilterCondition(scheduleTrackerSearch);
        //                if(scheduleTrackers != null && scheduleTrackers.Count() > 0 && scheduleTrackers.FirstOrDefault() != null)
        //                {
        //                    response.BCSeatsAvailable = scheduleTrackers.FirstOrDefault().BCSeatsRemaining;
        //                    response.NBCSeatsAvailable = scheduleTrackers.FirstOrDefault().NBCSeatsRemaining;
        //                }
        //                //add the prepared response to final list
        //                airlinesSearchResponse.Add(response);
        //            }
        //        }
        //    }
        //    return airlinesSearchResponse;
        //}
        //[HttpPost]
        //[Route("CheckForAvailableSeatsAndAddTracker")]
        //public BookingStatusCode CheckForAvailableSeatsAndAddTracker([FromBody] AirlineScheduleTracker airlineScheduleTracker)
        //{
        //    if (AirlinesValidation.ValidateCheckForAvailableSeats(airlineScheduleTracker))
        //        throw new Exception("AirlinesValidation.ValidateCheckForAvailableSeats Failed");

        //    if (airlineScheduleTracker.BCSeatsRemaining < 0)
        //        airlineScheduleTracker.BCSeatsRemaining = 0;
        //    if (airlineScheduleTracker.NBCSeatsRemaining < 0)
        //        airlineScheduleTracker.NBCSeatsRemaining = 0;

        //    BookingStatusCode response = BookingStatusCode.Invalid;
        //    //validate if scheduleId exist
        //    var schedules = airlineRepo.GetAirlineSchedules(airlineScheduleTracker.ScheduleId);
        //    if(schedules != null && schedules.Count() > 0 && schedules.FirstOrDefault() != null)
        //    {
        //        //validate if airline is active
        //        var schedule = schedules.FirstOrDefault();
        //        if (!schedules.FirstOrDefault().Airline.IsDeleted && schedules.FirstOrDefault().Airline.IsActive)
        //        {
        //            //prepare search request - just to be sure that not passing and filtering by any other parameters
        //            var scheduleTrackerSearch = new AirlineScheduleTracker()
        //            {
        //                ScheduleId = schedule.Id,
        //                ActualDepartureDate = airlineScheduleTracker.ActualDepartureDate
        //            };
        //            //check if tracker already exists
        //            var scheduleTrackers = airlineRepo.GetAirlineScheduleTrackerByFilterCondition(scheduleTrackerSearch);
        //            if(scheduleTrackers != null && scheduleTrackers.Count() > 0 && scheduleTrackers.FirstOrDefault() != null)
        //            {
        //                var tracker = scheduleTrackers.FirstOrDefault();
        //                //check seats are available
        //                if ((airlineScheduleTracker.BCSeatsRemaining <= 0 ||
        //                        tracker.BCSeatsRemaining >= airlineScheduleTracker.BCSeatsRemaining)
        //                    && (airlineScheduleTracker.NBCSeatsRemaining <= 0 ||
        //                        tracker.NBCSeatsRemaining >= airlineScheduleTracker.NBCSeatsRemaining))
        //                {
        //                    bool result = airlineRepo.UpdateAirlineScheduleTracker(tracker.Id, 
        //                        airlineScheduleTracker.BCSeatsRemaining, airlineScheduleTracker.NBCSeatsRemaining);
        //                    if(result)
        //                    {
        //                        response = BookingStatusCode.Booked;
        //                    }
        //                }
        //                else
        //                {
        //                    //seats are not available
        //                    response = BookingStatusCode.Waiting;
        //                }
        //            }
        //            else
        //            {
        //                //add new tracker
        //                if ((airlineScheduleTracker.BCSeatsRemaining <= 0 ||
        //                        schedule.Airline.TotalBCSeats >= airlineScheduleTracker.BCSeatsRemaining)
        //                    && (airlineScheduleTracker.NBCSeatsRemaining <= 0 ||
        //                        schedule.Airline.TotalNBCSeats >= airlineScheduleTracker.NBCSeatsRemaining))
        //                {
        //                    var tracker = new AirlineScheduleTracker()
        //                    {
        //                        ScheduleId = schedule.Id,
        //                        BCSeatsRemaining = schedule.Airline.TotalBCSeats - airlineScheduleTracker.BCSeatsRemaining,
        //                        NBCSeatsRemaining = schedule.Airline.TotalNBCSeats - airlineScheduleTracker.NBCSeatsRemaining
        //                    };
        //                    if (!schedule.IsRegular)
        //                    {
        //                        tracker.ActualDepartureDate = schedule.DepartureDate.Value.Date;
        //                        tracker.ActualArrivalDate = schedule.ArrivalDate.Value.Date;
        //                    }
        //                    else
        //                    {
        //                        var today = DateTime.Now.DayOfWeek;
        //                        if ((int)schedule.DepartureDay.Value >= (int)today)
        //                            tracker.ActualDepartureDate = DateTime.Now.AddDays((int)schedule.DepartureDay.Value - (int)today);
        //                        else
        //                        {
        //                            tracker.ActualDepartureDate = DateTime.Now.AddDays(
        //                                ((int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)today)
        //                                + (int)schedule.DepartureDay.Value + 1);
        //                        }
        //                        if ((int)schedule.ArrivalDay.Value >= (int)today)
        //                            tracker.ActualArrivalDate = DateTime.Now.AddDays((int)schedule.ArrivalDay.Value - (int)today);
        //                        else
        //                        {
        //                            tracker.ActualArrivalDate = DateTime.Now.AddDays(
        //                                ((int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)today)
        //                                + (int)schedule.ArrivalDay.Value + 1);
        //                        }
        //                    }
        //                    tracker.ActualDepartureDate = tracker.ActualDepartureDate +
        //                        new TimeSpan(schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
        //                    tracker.ActualArrivalDate = tracker.ActualArrivalDate +
        //                        new TimeSpan(schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
        //                    tracker.IsDeleted = false;
        //                    long trackerId = airlineRepo.AddAirlineScheduleTracker(tracker);
        //                    if (trackerId > 0)
        //                    {
        //                        //seats are available
        //                        response = BookingStatusCode.Booked;
        //                    }
        //                }
        //                else
        //                {
        //                    //invalid seats booking
        //                }
        //            }
        //        }
        //    }
        //    return response;
        //}
        //[HttpPost]
        //[Route("RevertScheduleTracker")]
        //public bool RevertScheduleTracker([FromBody] AirlineScheduleTracker airlineScheduleTracker)
        //{
        //    if (AirlinesValidation.RevertScheduleTracker(airlineScheduleTracker))
        //        throw new Exception("AirlinesValidation.RevertScheduleTracker Failed");

        //    if (airlineScheduleTracker.BCSeatsRemaining < 0)
        //        airlineScheduleTracker.BCSeatsRemaining = 0;
        //    if (airlineScheduleTracker.NBCSeatsRemaining < 0)
        //        airlineScheduleTracker.NBCSeatsRemaining = 0;

        //    bool result = true;
        //    //prepare search request - just to be sure that not passing and filtering by any other parameters
        //    var scheduleTrackerSearch = new AirlineScheduleTracker()
        //    {
        //        ScheduleId = airlineScheduleTracker.ScheduleId,
        //        ActualDepartureDate = airlineScheduleTracker.ActualDepartureDate
        //    };
        //    //get existing tracker
        //    var scheduleTrackers = airlineRepo.GetAirlineScheduleTrackerByFilterCondition(scheduleTrackerSearch);
        //    if (scheduleTrackers != null && scheduleTrackers.Count() > 0 && scheduleTrackers.FirstOrDefault() != null)
        //    {
        //        var tracker = scheduleTrackers.FirstOrDefault();
        //        if(!tracker.AirlineSchedule.Airline.IsDeleted && tracker.AirlineSchedule.Airline.IsActive)
        //        {
        //            if((tracker.BCSeatsRemaining + airlineScheduleTracker.BCSeatsRemaining <= tracker.AirlineSchedule.Airline.TotalBCSeats)
        //                && (tracker.NBCSeatsRemaining + airlineScheduleTracker.NBCSeatsRemaining <= tracker.AirlineSchedule.Airline.TotalNBCSeats))
        //            {
        //                result = airlineRepo.UpdateAirlineScheduleTracker(tracker.Id,
        //                airlineScheduleTracker.BCSeatsRemaining, airlineScheduleTracker.NBCSeatsRemaining, true);
        //            }
        //        }
        //    }
        //    return result;
        //}
    }
}

using AirlinesDTOs;
using CommonDTOs;
using Flight.Airlines.Models.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ServiceContracts.Airlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Flight.Airlines.Controllers
{
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class AirlinesController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IAirlinesRepository airlineRepo;
        //private readonly ActionExecutedContext context;
        //private static object syslockObj = new object ();
        //private readonly HeaderInfo headerInfo;
        //private static HeaderInfo headerInfo
        //{
        //    get
        //    {
        //        return _headerInfo;
        //    }
        //    set
        //    {
        //        if(_headerInfo != null)
        //        {
        //            lock(syslockObj)
        //            {
        //                _headerInfo = SetHeaderInfo();
        //            }
        //        }
        //    }
        //}

        public AirlinesController(IConfiguration config, IAirlinesRepository airlinesRepo)
        {
            this.config = config;
            this.airlineRepo = airlinesRepo;
            //this.context = context;
            //headerInfo = SetHeaderInfo();
        }

        //[NonAction]
        //private HeaderInfo SetHeaderInfo()
        //{
        //    return  new HeaderInfo()
        //    {
        //        UserId = context?.HttpContext?.Request?.Headers["UserId"],
        //        TenantId = context?.HttpContext?.Request?.Headers["TenantId"],
        //        AccessToken = context?.HttpContext?.Request?.Headers["AccessToken"]
        //    };
        //}

        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "AirlinesController -> Pong";
        }

        [HttpGet]
        public IEnumerable<AirlinesDTOs.Airlines> Get()
        {
            return airlineRepo.GetAirlines();
        }

        [HttpGet]
        [Route("{id}")]
        [Route("GetAirlines/{id}")]
        public IEnumerable<AirlinesDTOs.Airlines> Get(long id)
        {
            return airlineRepo.GetAirlines(id);
        }

        [HttpPost]
        [Route("GetAirlinesByFiltercondition")]
        public IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition([FromBody] AirlinesDTOs.AirlineDetails airline)
        {
            return airlineRepo.GetAirlinesByFiltercondition(airline);
        }

        [HttpPost]
        [Route("Add")]
        public long Add([FromBody] AirlinesDTOs.Airlines airline)
        {
            if (!AirlinesValidation.ValidateAddFlight(airline))
                throw new Exception("AirlinesValidation.ValidateAddFlight Falied");

            if (airlineRepo.IsAirlineAlreadyExists(airline))
                throw new Exception("Airline name and/or code already exists");

            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            airline.Createdby = Convert.ToInt64(userId);
            airline.ModifiedBy = Convert.ToInt64(userId);
            return airlineRepo.AddAirline(airline);
        }

        [HttpPost]
        [Route("Update")]
        public Result Update([FromBody] AirlinesDTOs.AirlineDetails airline)
        {
            if (!AirlinesValidation.ValidateUpdateFlight(airline))
                throw new Exception("AirlinesValidation.ValidateUpdateFlight Falied");

            AirlinesDTOs.Airlines airline_1 = new AirlinesDTOs.Airlines()
            {
                Id = airline.Id,
                Name = airline.Name,
                AirlineCode = airline.AirlineCode,
                ContactNumber = airline.ContactNumber,
                ContactAddress = airline.ContactAddress,
                TotalSeats = airline.TotalBCSeats,
                TotalBCSeats = airline.TotalBCSeats,
                TotalNBCSeats = airline.TotalNBCSeats,
                BCTicketCost = airline.BCTicketCost,
                NBCTicketCost = airline.NBCTicketCost
            };
            if (airline.IsActive != null)
                airline_1.IsActive = (bool)airline.IsActive;
            if (airlineRepo.IsAirlineAlreadyExists(airline_1))
                throw new Exception("Airline name and/or code already exists");

            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            return airlineRepo.UpdateAirline(airline, userId);
        }

        [HttpPost]
        [Route("ActivateDeactivateAirline")]
        public Result ActivateDeactivateAirline([FromBody] dynamic obj)
        {
            //long id = Convert.ToInt64(airline["Id"].ToString());
            if (!AirlinesValidation.ValidateActivateDeactivateAirline(obj.GetProperty("Id"), obj.GetProperty("IsActive")))
                throw new Exception("AirlinesValidation.ValidateActivateDeactivateAirline Falied");

            AirlinesDTOs.Airlines airline = new AirlinesDTOs.Airlines()
            {
                Id = Convert.ToInt64(obj.GetProperty("Id").ToString()),
                IsActive = Convert.ToBoolean(obj.GetProperty("IsActive").ToString().Trim()),
                ModifiedBy = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"])
            };
            return airlineRepo.ActivateDeactivateAirline(airline);
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
            return airlineRepo.DeleteAirline(airline);
        }

        [HttpPost]
        [Route("PermanentDelete")]
        public Result PermanentDelete([FromBody] long id)
        {
            return airlineRepo.PermanentDeleteAirline(id);
        }

        [HttpPost]
        [Route("MapAirlinesDiscountTags")]
        public bool MapAirlinesDiscountTags([FromBody] List<AirlinesDTOs.AirlineDiscountTagMappingDetails> airlineDiscountTagMappingDetails)
        {
            if (!AirlinesValidation.ValidateAirlineDiscountTagMappings(airlineDiscountTagMappingDetails))
                throw new Exception("AirlinesValidation.ValidateAirlineDiscountTagMappings Falied");

            List<AirlineDiscountTagMappings> airlineDiscountTagMappings = new List<AirlineDiscountTagMappings>();
            var mappings = airlineDiscountTagMappingDetails.Where(x => x.Airline != null
                && (x.Airline.Id > 0 || !string.IsNullOrWhiteSpace(x.Airline.Name) || !string.IsNullOrWhiteSpace(x.Airline.AirlineCode))
                && x.DiscountTags != null);
            if (mappings != null && mappings.Count() > 0)
            {
                long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
                foreach (var map in mappings)
                {
                    var airline = airlineRepo.GetAirlinesByFiltercondition(map.Airline);
                    if (airline != null && airline.Count() > 0)
                    {
                        var discountTags = map.DiscountTags.Where(x => x.Id > 0
                                || !string.IsNullOrWhiteSpace(x.Name) || !string.IsNullOrWhiteSpace(x.DiscountCode));
                        if (discountTags != null && discountTags.Count() > 0)
                        {
                            var discountTagDetails = airlineRepo.GetDiscountTagsByMultipleFilterconditions(discountTags.ToList());
                            if (discountTagDetails != null && discountTagDetails.Count() > 0)
                            {
                                airlineDiscountTagMappings.AddRange(discountTagDetails.
                                    Select(x => new AirlineDiscountTagMappings()
                                    {
                                        AirlineId = airline.FirstOrDefault().Id,
                                        DiscountTagId = x.Id,
                                        TaggedBy = userId,
                                        Airline = null,
                                        DiscountTag = null
                                    }));
                            }
                        }
                    }
                }

                if (airlineDiscountTagMappings == null || airlineDiscountTagMappings.Count() <= 0)
                    throw new Exception("All input mappings are invalid");

                return airlineRepo.AddAirlineDiscountTagMappings(airlineDiscountTagMappings);
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
                throw new Exception("AirlinesValidation.ValidateRemapAirlineDiscountTagsDetails Falied");

            List<AirlineDiscountTagMappings> addedAirlineDiscountTagMappings = new List<AirlineDiscountTagMappings>();
            List<AirlineDiscountTagMappings> removedAirlineDiscountTagMappings = new List<AirlineDiscountTagMappings>();
            var mappings = remapAirlineDiscountTagsDetails.Where(x => x.AirlineId > 0
                && (x.AddedDiscountTagIds != null || x.RemovedDiscountTagIds != null));
            if (mappings != null && mappings.Count() > 0)
            {
                long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
                foreach (var map in mappings)
                {
                    var airline = airlineRepo.GetAirlines(map.AirlineId);
                    if (airline != null && airline.Count() > 0 && airline.FirstOrDefault() != null)
                    {
                        if (map.AddedDiscountTagIds != null && map.AddedDiscountTagIds.Count() > 0)
                        {
                            var addedDiscountTagIds = map.AddedDiscountTagIds.Except(map.RemovedDiscountTagIds ?? new List<long>());
                            if (addedDiscountTagIds != null && addedDiscountTagIds.Count() > 0)
                            {
                                var discountTagDetails = airlineRepo.GetDiscountTagByIds(addedDiscountTagIds.ToList());
                                if (discountTagDetails != null && discountTagDetails.Count() > 0)
                                {
                                    addedAirlineDiscountTagMappings.AddRange(discountTagDetails.
                                        Select(x => new AirlineDiscountTagMappings()
                                        {
                                            AirlineId = airline.FirstOrDefault().Id,
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
                    throw new Exception("All input mappings are invalid");

                if (addedAirlineDiscountTagMappings != null && addedAirlineDiscountTagMappings.Count() > 0)
                    result = airlineRepo.AddAirlineDiscountTagMappings(addedAirlineDiscountTagMappings);
                if (removedAirlineDiscountTagMappings != null && removedAirlineDiscountTagMappings.Count() > 0)
                    result = airlineRepo.RemoveAirlineDiscountTagMappings(removedAirlineDiscountTagMappings);
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
            var airlineDiscountTagsMappings = airlineRepo.GetAirlineDiscountTagsMappings();
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
                throw new Exception("Atleast one of the fields (airlineId or discountId) should be be greated than zero");

            var airlineDiscountTagsMappingDetails = new List<AirlineDiscountTagMappingDetails>();
            var airlineDiscountTagsMappings = airlineRepo.GetAirlineDiscountTagsMappings(airlineId, discountId);
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

        //Search available flights
        [HttpPost]
        [Route("GetAvailableAirlines")]
        public IEnumerable<AirlinesSearchResponse> GetAvailableAirlines([FromBody] AirlinesSearchRequest airlinesSearchRequest)
        {
            if (airlinesSearchRequest.DepartureDate == null)
                airlinesSearchRequest.DepartureDate = DateTime.Now.Date;
            if (airlinesSearchRequest.ArrivalDate == null)
                airlinesSearchRequest.ArrivalDate = DateTime.Now.AddDays(7).Date;
            if (!AirlinesValidation.ValidateGetAvailableAirlines(airlinesSearchRequest))
                throw new Exception("AirlinesValidation.ValidateGetAvailableAirlines Failed");

            var airlinesSearchResponse = new List<AirlinesSearchResponse>();
            //get available airline schedules
            var scheduleSearch = new AirlineScheduleDetails()
            {
                From = airlinesSearchRequest.From,
                To = airlinesSearchRequest.To,
                DepartureDay = airlinesSearchRequest.DepartureDate.Value.DayOfWeek,
                DepartureDate = airlinesSearchRequest.DepartureDate.Value.Date
            };
            var availableSchedules = airlineRepo.GetGetAirlineSchedulesByFilterCondition(scheduleSearch);
            if(availableSchedules != null && availableSchedules.Count() > 0)
            {
                //get active airline schedules
                var activeSchedules = availableSchedules.Where(x => x.Airline != null && !x.Airline.IsDeleted && x.Airline.IsActive);
                if(activeSchedules != null && activeSchedules.Count() > 0)
                {
                    //get available seats
                    foreach (var schedule in activeSchedules)
                    {
                        var response = new AirlinesSearchResponse()
                        {
                            AirlineSchedules = schedule,
                            DiscountTags = null,
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
                            if((int)schedule.DepartureDay.Value >= (int)today)
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
                        var discountTagMappings = airlineRepo.GetAirlineDiscountTagsMappings(schedule.AirlineId);
                        if(discountTagMappings != null && discountTagMappings.Count() > 0)
                        {
                            var activeDiscountTags = discountTagMappings.Where(x => !x.DiscountTag.IsDeleted && x.DiscountTag.IsActive);
                            if(activeDiscountTags != null && activeDiscountTags.Count() > 0)
                            {
                                response.DiscountTags = activeDiscountTags.Select(x => x.DiscountTag).ToList();
                            }
                        }
                        //get available seats
                        var scheduleTrackerSearch = new AirlineScheduleTracker()
                        {
                            ScheduleId = schedule.Id,
                            ActualDepartureDate = airlinesSearchRequest.DepartureDate.Value.Date
                        };
                        var scheduleTrackers = airlineRepo.GetAirlineScheduleTrackerByFilterCondition(scheduleTrackerSearch);
                        if(scheduleTrackers != null && scheduleTrackers.Count() > 0 && scheduleTrackers.FirstOrDefault() != null)
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
    }
}

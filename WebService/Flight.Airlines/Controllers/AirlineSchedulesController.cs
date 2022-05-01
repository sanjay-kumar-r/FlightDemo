using AirlinesDTOs;
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


namespace Flight.Airlines.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class AirlineSchedulesController : ControllerBase
    {
        private readonly CustomSettings customSettings;
        //private readonly IConfiguration config;
        private readonly IAirlinesRepository airlinesRepo;
        private readonly ILogger logger;

        public AirlineSchedulesController(IConfiguration config, IAirlinesRepository airlinesRepo, ILogger logger)
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
            return "AirlineSchedulesController -> Pong";
        }

        [HttpGet]
        public IEnumerable<AirlinesDTOs.AirlineSchedules> Get()
        {
            return airlinesRepo.GetAirlineSchedules();
        }

        [HttpGet]
        [Route("{id}")]
        //[Route("GetAirlineSchedules/{id}")]
        public IEnumerable<AirlinesDTOs.AirlineSchedules> Get(long id)
        {
            return airlinesRepo.GetAirlineSchedules(id);
        }

        [HttpGet]
        [Route("GetAirlineSchedulesByAirlineId/{id}")]
        public IEnumerable<AirlinesDTOs.AirlineSchedules> GetAirlineSchedulesByAirlineId(long id)
        {
            return airlinesRepo.GetAirlineSchedules(id, true);
        }

        [HttpPost]
        [Route("GetAirlineSchedulesByFilterCondition")]
        public IEnumerable<AirlinesDTOs.AirlineSchedules> GetAirlineSchedulesByFilterCondition([FromBody] AirlinesDTOs.AirlineScheduleDetails schedule)
        {
            return airlinesRepo.GetGetAirlineSchedulesByFilterCondition(schedule);
        }

        [HttpPost]
        [Route("Add")]
        public long Add([FromBody] AirlineSchedules schedule)
        {
            if (!AirlinesValidation.ValidateAddAirlineSchedule(schedule))
                throw new Exception("AirlinesValidation.ValidateAddAirlineSchedule Falied");

            var apirline = airlinesRepo.GetAirlines(schedule.AirlineId);
            if(apirline == null || apirline.Count() <= 0 || apirline.FirstOrDefault() == null)
                throw new Exception("Invalid AirlineId");

            if (airlinesRepo.IsAirlineScheduleAlreadyExists(schedule))
                throw new Exception("AirlineSchedule details already exists");

            long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
            schedule.Createdby = userId;
            schedule.ModifiedBy = userId;
            return airlinesRepo.AddAirlineSchedule(schedule);
        }

        [HttpPost]
        [Route("ReSchedulesAirlinesByRange")]
        public List<long> ReSchedulesAirlinesByRange([FromBody] List<AirlineSchedules> schedules)
        {
            if (!AirlinesValidation.ValidateAddAirlineScheduleByRangs(schedules))
                throw new Exception("AirlinesValidation.ValidateAddAirlineSchedule Falied");

            long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
            var airlineIds = schedules.Select(x => x.AirlineId).ToList();
            var airlines = airlinesRepo.GetAirlinesbyIds(airlineIds);
            if (airlines != null && airlines.Count() > 0)
            {
                var validschedules = schedules.Where(x => airlines.Any(y => y.Id == x.AirlineId));

                //delete existing mappings with same scheduleIds and AirlineIds if any
                bool result = true;
                var scheduleIds = validschedules.Where(x => x.Id > 0).Select(x => x.Id).ToList();
                if (scheduleIds != null && scheduleIds.Count() > 0)
                    result = airlinesRepo.DeleteAirlineScheduleByScheduleIds(scheduleIds, userId);
                var scheduledAirlineIds = validschedules.Where(x => x.Id <= 0).Select(x => x.AirlineId).ToList();
                if (scheduledAirlineIds != null && scheduledAirlineIds.Count() > 0)
                {
                    var airlineSchedules = airlinesRepo.GetAirlineSchedulesByIds(scheduledAirlineIds, true);
                    if(airlineSchedules != null && airlineSchedules.Count() > 0)
                    {
                        scheduleIds = airlineSchedules.Select(x => x.Id).ToList();
                        result = airlinesRepo.DeleteAirlineScheduleByScheduleIds(scheduleIds, userId);
                    }
                }
                if (result)
                {
                    foreach (var schedule in validschedules)
                    {
                        schedule.Createdby = userId;
                        schedule.ModifiedBy = userId;
                        schedule.CreatedOn = DateTime.Now;
                        schedule.ModifiedOn = DateTime.Now;
                        schedule.Airline = null;
                    }
                    return airlinesRepo.AddAirlineSchedulesByRange(validschedules.ToList());
                }
                else
                    throw new Exception("Error while deleting existing mappings");
            }
            else
                throw new Exception("All AirlineIds are Invalid");
        }

        [HttpPost]
        [Route("Delete")]
        public bool DeleteAirlineSchedule([FromBody] long id)
        {
            if (id <= 0)
                throw new Exception("Validate DeleteAirlineSchedule Falied");


            long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
            return airlinesRepo.DeleteAirlineSchedule(id, userId);
        }

        [HttpPost]
        [Route("DeleteByScheduleIds")]
        public bool DeleteAirlineScheduleByIds([FromBody] List<long> ids)
        {
            if (ids == null || ids.Count() <= 0)
                throw new Exception("Validate DeleteAirlineScheduleByIds Falied");

            long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
            return airlinesRepo.DeleteAirlineScheduleByScheduleIds(ids, userId);
        }

        [HttpPost]
        [Route("DeleteByAirlineIds")]
        public bool DeleteAirlineScheduleByAirlineIds([FromBody] List<long> ids)
        {
            if (ids == null || ids.Count() <= 0)
                throw new Exception("Validate DeleteAirlineScheduleByIds Falied");

            bool result = false;
            long userId = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"]);
            var airlineSchedules = airlinesRepo.GetAirlineSchedulesByIds(ids, true);
            if (airlineSchedules != null && airlineSchedules.Count() > 0)
            {
                var scheduleIds = airlineSchedules.Select(x => x.Id).ToList();
                result = airlinesRepo.DeleteAirlineScheduleByScheduleIds(scheduleIds, userId);
            }
            return result;
        }
    }
}

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

        public AirlinesController(IConfiguration config, IAirlinesRepository usersRepo)
        {
            this.config = config;
            this.airlineRepo = usersRepo;
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
        public IEnumerable<AirlinesDTOs.Airlines> GetAirlinesByFiltercondition([FromBody] AirlinesDTOs.Airlines airline)
        {
            return airlineRepo.GetAirlinesByFiltercondition(airline);
        }

        [HttpPost]
        [Route("Add")]
        public long Add([FromBody] AirlinesDTOs.Airlines airline)
        {
            if (!AirlinesValidation.ValidateAddFlight(airline))
                throw new Exception("AirlinesValidation.ValidateAddFlight Falied");

            if (airlineRepo.FlightExists(airline))
                throw new Exception("Airline name and/or code already exists");

            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            airline.Createdby = Convert.ToInt64(userId);
            airline.ModifiedBy = Convert.ToInt64(userId);
            return airlineRepo.AddAirline(airline);
        }

        [HttpPost]
        [Route("Update")]
        public Result Update([FromBody] AirlinesDTOs.Airlines airline)
        {
            if (!AirlinesValidation.ValidateUpdateFlight(airline))
                throw new Exception("AirlinesValidation.ValidateAddFlight Falied");

            if (airlineRepo.FlightExists(airline))
                throw new Exception("Airline name and/or code already exists");

            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            airline.ModifiedBy = Convert.ToInt64(userId);
            return airlineRepo.UpdateAirline(airline);
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
            return airlineRepo.PermanentDelete(id);
        }
    }
}

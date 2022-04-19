using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GateWayController : ControllerBase
    {
        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "GateWayController -> Pong";
        }
    }
}

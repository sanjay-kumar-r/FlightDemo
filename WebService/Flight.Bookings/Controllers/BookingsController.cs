using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceContracts.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Flight.Bookings.Controllers
{
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IBookingsRepository bookingsRepo;
        public BookingsController(IConfiguration config, IBookingsRepository bookingsRepo)
        {
            this.config = config;
            this.bookingsRepo = bookingsRepo;
        }

        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "BookingsController -> Pong";
        }

        //[HttpGet]
        //public IEnumerable<UserDtOs.Users> Get()
        //{
        //    return bookingsRepo.GetUsers();
        //}
    }
}

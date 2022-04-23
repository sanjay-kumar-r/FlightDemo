using Flight.Users.Model.Utils;
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

        [HttpGet]
        public IEnumerable<BookingsDTOs.Bookings> Get()
        {
            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            return bookingsRepo.GetBookings(userId);
        }

        [HttpGet]
        [Route("{id}")]
        [Route("GetBookingsById/{id}")]
        public IEnumerable<BookingsDTOs.Bookings> Get(int id)
        {
            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            return bookingsRepo.GetBookings(userId, id);
        }

        [HttpPost]
        [Route("GetBookingsByFilterCondition/{id}")]
        public IEnumerable<BookingsDTOs.Bookings> GetBookingsByFiltercondition(BookingsDTOs.Bookings booking)
        {
            booking.UserId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            return bookingsRepo.GetBookingsByFiltercondition(booking);
        }

        [HttpPost]
        [Route("BookTicket")]
        public string BookTicket(BookingsDTOs.Bookings booking)
        {
            if (!BookingsValidation.ValidateBookTicket(booking))
                throw new Exception("BookingsValidation.ValidateBookTicket Falied");

            //check scheduleId is not deleted and
            //corresponding airlineId is not deleted and is active
            //and seats are available - api call


            booking.UserId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            long id = bookingsRepo.BookTicket(booking);
            return null;
        }
    }
}

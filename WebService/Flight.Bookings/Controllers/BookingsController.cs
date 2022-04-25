using AirlinesDTOs;
using BookingsDTOs;
using CommonDTOs;
using CommonUtils.APIExecuter;
using Flight.Bookings.Models.Utils;
using Flight.Users.Model.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceContracts.Bookings;
using ServiceContracts.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace Flight.Bookings.Controllers
{
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly CustomSettings customSettings;
        private readonly IConfiguration config;
        private readonly IBookingsRepository bookingsRepo;
        private readonly ILogger logger;
        public BookingsController(IConfiguration config, IBookingsRepository bookingsRepo, ILogger logger)
        {
            customSettings = new CustomSettings();
            config.GetSection("CustomSettings").Bind(customSettings);
            this.config = config;
            this.bookingsRepo = bookingsRepo;
            this.logger = logger;
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
        public async Task<BookingResponse> BookTicket([FromBody] BookingsDTOs.Bookings booking)
        {
            if (!BookingsValidation.ValidateBookTicket(booking))
                throw new Exception("BookingsValidation.ValidateBookTicket Falied");

            HeaderInfo headerInfo = new HeaderInfo()
            {
                UserId = HttpContext.Request.Headers["UserId"],
                TenantId = HttpContext.Request.Headers["TenantId"],
                AccessToken = HttpContext.Request.Headers["AccessToken"]
            };
            BookingStatusCode status = BookingStatusCode.Invalid;
            try
            {
                string pnr = string.Empty;
                string checkForAvailableSeatsAndAddTrackerUrl = customSettings.EndpointUrls["CheckForAvailableSeatsAndAddTrackerUrl"];
                string apiGatewayBaseUrl = customSettings.ApiGatewayBaseUrl;
                string requestUrl = apiGatewayBaseUrl.Trim('/', ' ') + "/" + checkForAvailableSeatsAndAddTrackerUrl.Trim('/', ' ');
                var tracker = new AirlineScheduleTracker()
                {
                    ScheduleId = booking.ScheduleId,
                    ActualDepartureDate = booking.DateBookedFor,
                    BCSeatsRemaining = booking.BCSeats,
                    NBCSeatsRemaining = booking.NBCSeats,
                };
                using (var response = await ApiExecutor.ExecutePostAPI(requestUrl, headerInfo, tracker))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    status = (BookingStatusCode)Convert.ToInt32(apiResponse);
                }
                //using (var httpClient = new HttpClient())
                //{
                //    string checkForAvailableSeatsAndAddTrackerUrl = customSettings.EndpointUrls["CheckForAvailableSeatsAndAddTrackerUrl"];
                //    string apiGatewayBaseUrl = customSettings.ApiGatewayBaseUrl;
                //    string requestUrl = apiGatewayBaseUrl.Trim('/', ' ') + "/" + checkForAvailableSeatsAndAddTrackerUrl.Trim('/', ' ');
                //    httpClient.BaseAddress = new Uri(requestUrl);
                //    httpClient.DefaultRequestHeaders.Clear();
                //    httpClient.DefaultRequestHeaders.Add("UserId", headerInfo.UserId);
                //    httpClient.DefaultRequestHeaders.Add("TenantId", headerInfo.TenantId);
                //    httpClient.DefaultRequestHeaders.Add("AccessToken", headerInfo.AccessToken);
                //    var tracker = new AirlineScheduleTracker()
                //    {
                //        ScheduleId = booking.ScheduleId,
                //        ActualDepartureDate = booking.DateBookedFor,
                //        BCSeatsRemaining = booking.BCSeats,
                //        NBCSeatsRemaining = booking.NBCSeats,
                //    };
                //    //var buffer = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tracker));
                //    //var byteContent = new ByteArrayContent(buffer);
                //    //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //    //HttpResponseMessage response = httpClient.PostAsJsonAsync(requestUrl, tracker).Result;
                //    using (var response = await httpClient.PostAsJsonAsync(requestUrl, tracker))
                //    {
                //        string apiResponse = await response.Content.ReadAsStringAsync();
                //        status = (BookingStatusCode)Convert.ToInt32(apiResponse);
                //    }
                //}

                //using (var httpClient = new HttpClient())
                //{
                //    string checkForAvailableSeatsAndAddTrackerUrl = customSettings.EndpointUrls["CheckForAvailableSeatsAndAddTrackerUrl"];
                //    string apiGatewayBaseUrl = customSettings.ApiGatewayBaseUrl;
                //    string requestUrl = apiGatewayBaseUrl.Trim('/', ' ') + "/" + checkForAvailableSeatsAndAddTrackerUrl.Trim('/', ' ');
                //    var tracker = new AirlineScheduleTracker()
                //    {
                //        ScheduleId = booking.ScheduleId,
                //        ActualDepartureDate = booking.DateBookedFor,
                //        BCSeatsRemaining = booking.BCSeats,
                //        NBCSeatsRemaining = booking.NBCSeats,
                //    };
                //    string body = JsonConvert.SerializeObject(tracker);
                //    StringContent requestBody = new StringContent(body, Encoding.UTF8, "application/json");
                //    using (var response = await httpClient.PostAsync(requestUrl, requestBody))
                //    {
                //        string apiResponse = await response.Content.ReadAsStringAsync();
                //        status = (BookingStatusCode)Convert.ToInt32(apiResponse);
                //    }
                //}
                if (!status.Equals(BookingStatusCode.Invalid))
                {
                    if (status.Equals(BookingStatusCode.Booked))
                    {
                        Guid g = Guid.NewGuid();
                        pnr = g.ToString();
                        booking.PNR = pnr;
                    }
                    booking.UserId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
                    booking.BookingStatusId = (int)status;
                    bookingsRepo.BookTicket(booking);
                }
                else
                    logger.Log(LogLevel.INFO, "Invalid booking (wrong schedule and/or daparturedate)");
                BookingResponse bookingResponse = new BookingResponse()
                {
                    BookingStatus = status.ToString(),
                    PNR = pnr
                };
                logger.Log(LogLevel.INFO, $"Booking successful with response ='{JsonConvert.SerializeObject(bookingResponse)}'");
                return bookingResponse;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, $"Error while Booking Ticket. Error message => '{ex.Message}' , Stack Trace => {ex.StackTrace}");
                logger.Log(LogLevel.INFO, "Reverting Schedule tracker changes(updating back the available seats)");
                BookingHelper bookingHelper = new BookingHelper(config, bookingsRepo, logger, headerInfo);
                bool result = await bookingHelper.BookingRevert(booking);
                if(result)
                {
                    BookingResponse bookingResponse = new BookingResponse()
                    {
                        BookingStatus = BookingStatusCode.Invalid.ToString()
                    };
                    logger.Log(LogLevel.INFO, $"Reverting booking schedule with response ='{BookingStatusCode.Invalid.ToString()}'");
                    return bookingResponse;
                }
                else
                    throw new Exception("Error while reverting booking");
            }
        }

        [HttpPost]
        [Route("CancelBooking")]
        public async Task<bool> CancelBooking([FromBody] int id)
        {
            HeaderInfo headerInfo = new HeaderInfo()
            {
                UserId = HttpContext.Request.Headers["UserId"],
                TenantId = HttpContext.Request.Headers["TenantId"],
                AccessToken = HttpContext.Request.Headers["AccessToken"]
            };
            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            var bookings = bookingsRepo.GetBookings(userId, id).ToList();
            bool result = false;
            if (bookings != null && bookings.Count() > 0 && bookings.FirstOrDefault() != null)
            {
                
                result = bookingsRepo.CancelTicketByBookingIds(new List<long>() { id }, userId);
                if (result)
                {
                    var booking = bookings.FirstOrDefault();
                    if (booking.BookingStatusId == (int)BookingStatusCode.Booked)
                    {
                        logger.Log(LogLevel.INFO, "Reverting Schedule tracker changes(updating back the available seats)");
                        BookingHelper bookingHelper = new BookingHelper(config, bookingsRepo, logger, headerInfo);
                        result = await bookingHelper.ScheduleTrackerRevert(booking);
                        if (result)
                        {
                            logger.Log(LogLevel.INFO, "Revert Schedule Tracker completed successfully");
                            //if(result)
                            //    logger.Log(LogLevel.INFO, "Cancel Booking completed successfully");
                            //logger.Log(LogLevel.INFO, "Enable other booking requests");
                            //var bookingSearch = new BookingsDTOs.Bookings()
                            //{
                            //    ScheduleId = booking.ScheduleId,
                            //    DateBookedFor = booking.DateBookedFor
                            //};
                            //bookingsRepo.GetBookingsByFiltercondition(bookingSearch);
                        }
                        else
                            logger.Log(LogLevel.ERROR, "Revert Schedule Tracker incomplete");
                    }
                }
            }
            logger.Log(LogLevel.INFO, $"CancelBooking for id='{id}' => result='{result}'");
            return result;
        }
    }
}

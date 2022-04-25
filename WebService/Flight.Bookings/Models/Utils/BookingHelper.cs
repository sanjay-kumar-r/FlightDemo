using AirlinesDTOs;
using BookingsDTOs;
using CommonDTOs;
using CommonUtils.APIExecuter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceContracts.Bookings;
using ServiceContracts.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Flight.Bookings.Models.Utils
{
    public class BookingHelper
    {
        private readonly CustomSettings customSettings;
        private readonly IConfiguration config;
        private readonly IBookingsRepository bookingsRepo;
        private readonly ILogger logger;
        //private readonly long userId ;
        private readonly HeaderInfo headerInfo;
        public BookingHelper(IConfiguration config, IBookingsRepository bookingsRepo, ILogger logger, HeaderInfo headerInfo)
        {
            customSettings = new CustomSettings();
            config.GetSection("CustomSettings").Bind(customSettings);
            //this.config = config;
            this.bookingsRepo = bookingsRepo;
            this.logger = logger;
            this.headerInfo = headerInfo;
        }
        public async Task<bool> BookingRevert(BookingsDTOs.Bookings booking)
        {
            bool result = true;
            logger.Log(LogLevel.INFO, "Reverting booking status to invalid");
            BookingsDTOs.Bookings bookingSearch = new BookingsDTOs.Bookings()
            {
                UserId = Convert.ToInt64(headerInfo.UserId),
                ScheduleId = booking.ScheduleId,
                DateBookedFor = booking.DateBookedFor,
                BookingStatusId = 0,
            };
            var bookings = bookingsRepo.GetBookingsByFiltercondition(bookingSearch);
            if (bookings != null && bookings.Count() > 0 && bookings.FirstOrDefault() != null)
            {
                var userBooking = bookings.FirstOrDefault();
                result = bookingsRepo.UpdateBookingStatus(userBooking.Id, userBooking.UserId, BookingStatusCode.Invalid);
            }
            if (result)
            {
                result = await ScheduleTrackerRevert(booking);
                if (result)
                    logger.Log(LogLevel.INFO, "Revert Schedule Tracker completed successfully");
                else
                    logger.Log(LogLevel.ERROR, "Revert Schedule Tracker incomplete");
            }
            else
                logger.Log(LogLevel.ERROR, "Revert Booking incomplete (error while reverting booking table)");
            return result;
        }

        public async Task<bool> ScheduleTrackerRevert(BookingsDTOs.Bookings booking)
        {
            bool result = true;
            logger.Log(LogLevel.INFO, "Reverting Schedule Tracker initiated :");
            string RevertScheduleTrackerUrl = customSettings.EndpointUrls["RevertScheduleTrackerUrl"];
            string apiGatewayBaseUrl = customSettings.ApiGatewayBaseUrl;
            string requestUrl = apiGatewayBaseUrl.Trim('/', ' ') + "/" + RevertScheduleTrackerUrl.Trim('/', ' ');
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
                result = Convert.ToBoolean(apiResponse);
            }
            //using (var httpClient = new HttpClient())
            //{
            //    string RevertScheduleTrackerUrl = customSettings.EndpointUrls["RevertScheduleTrackerUrl"];
            //    string apiGatewayBaseUrl = customSettings.ApiGatewayBaseUrl;
            //    string requestUrl = apiGatewayBaseUrl.Trim('/', ' ') + "/" + RevertScheduleTrackerUrl.Trim('/', ' ');
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
            //        result = Convert.ToBoolean(apiResponse);
            //    }
            //}
            return result;
        }
    }
}

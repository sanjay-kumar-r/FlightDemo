using CommonDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceContracts.Bookings;
using BookingsDTOs;

namespace Flight.Users.Model.Utils
{
    public class BookingsRepository : IBookingsRepository
    {
        private readonly BookingsDBContext context;

        public BookingsRepository(BookingsDBContext context)
        {
            this.context = context;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public IEnumerable<BookingsDTOs.Bookings> GetBookings(long userId, long? id = null)
        {
            return context.Bookings.Include(x => x.BookingStatus).AsNoTracking().Where(x => x.UserId == userId && (id == null || x.Id == id));
        }

        public IEnumerable<BookingsDTOs.Bookings> GetBookingsByFiltercondition(BookingsDTOs.Bookings booking)
        {
            return context.Bookings.Include(x => x.BookingStatus).AsNoTracking().Where(x => x.UserId == booking.UserId 
                && (booking.Id <= 0 || booking.Id == x.Id)
                && (booking.ScheduleId <= 0 || booking.ScheduleId == x.ScheduleId)
                && (booking.ScheduleId <= 0 || booking.ScheduleId == x.ScheduleId)
                && (booking.DateBookedFor != null || booking.DateBookedFor.Date == x.DateBookedFor.Date)
                && (!Enum.IsDefined(typeof(BookingStatusCode), booking.BookingStatusId) || booking.BookingStatusId == x.BookingStatusId)
                && (booking.CreatedOn != null || booking.CreatedOn.Value.Date == x.CreatedOn.Value.Date)
                && (booking.CanceledOn != null || booking.CanceledOn.Value.Date == x.CanceledOn.Value.Date)
                && (booking.IsRefunded != null || booking.IsRefunded == x.IsRefunded));
        }

        public long BookTicket(BookingsDTOs.Bookings booking)
        {
            booking.CreatedOn = DateTime.Now;
            booking.BookingStatusId = context.BookingStatus.First(x => x.Id == (int)BookingStatusCode.Booked).Id;
            context.Bookings.Add(booking);
            context.SaveChanges();
            long id = booking.Id;
            return id;
        }

        public bool CancelTicketByBookingIds(List<long> ids, long userId)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            bool result = false;
            if(ids != null && context.Bookings.Count() > 0)
            {
                foreach(int id in ids)
                {
                    var booking = context.Bookings.AsNoTracking().FirstOrDefault(x => x.Id == id && x.UserId == userId);
                    booking.BookingStatusId = (int)BookingStatusCode.Canceled;
                    var bookingUpdated = context.Bookings.Attach(booking);
                    bookingUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
    }
}

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
            return context.Bookings.Include(x => x.BookingStatus).AsEnumerable()
                .Where(x => x.UserId == userId && (id == null || x.Id == id)).ToList();
        }

        public IEnumerable<BookingsDTOs.Bookings> GetBookingsByFiltercondition(BookingsDTOs.Bookings booking)
        {
            return context.Bookings.Include(x => x.BookingStatus).AsEnumerable().Where(x => x.UserId == booking.UserId 
                && (booking.Id <= 0 || booking.Id == x.Id)
                && (booking.ScheduleId <= 0 || booking.ScheduleId == x.ScheduleId)
                && (booking.DateBookedFor == null || booking.DateBookedFor.Date == x.DateBookedFor.Date)
                && (!Enum.IsDefined(typeof(BookingStatusCode), booking.BookingStatusId) || booking.BookingStatusId == x.BookingStatusId)
                && (booking.CreatedOn == null || booking.CreatedOn.Value.Date == x.CreatedOn.Value.Date)
                && (booking.CanceledOn == null || booking.CanceledOn.Value.Date == x.CanceledOn.Value.Date)
                && (booking.IsRefunded == null || booking.IsRefunded == x.IsRefunded)).ToList();
        }

        public long BookTicket(BookingsDTOs.Bookings booking)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            long id = 0;
            if (booking != null)
            {
                var bookingExisting = context.Bookings.AsEnumerable().FirstOrDefault(x => x.UserId == booking.UserId 
                && x.DateBookedFor.Date == booking.DateBookedFor.Date 
                && x.DateBookedFor.ToShortTimeString().Equals(booking.DateBookedFor.ToShortTimeString()));
                if(bookingExisting != null && bookingExisting.Id > 0 && bookingExisting.BookingStatusId == (int)BookingStatusCode.Waiting)
                {
                    if(Enum.IsDefined(typeof(BookingStatusCode), booking.BookingStatusId))
                    {
                        id = bookingExisting.Id;
                        bookingExisting.BookingStatusId = (int)booking.BookingStatusId;
                        bookingExisting.PNR = booking.PNR;
                        var bookingUpdated = context.Bookings.Attach(bookingExisting);
                        bookingUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                else
                {
                    booking.CreatedOn = DateTime.Now;
                    booking.BookingStatusId = Enum.IsDefined(typeof(BookingStatusCode), booking.BookingStatusId) 
                        ? booking.BookingStatusId : (int)BookingStatusCode.Invalid;
                    context.Bookings.Add(booking);
                    context.SaveChanges();
                    id = booking.Id;
                    return id;
                }
            }
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
                    if (booking != null)
                    {
                        booking.BookingStatusId = (int)BookingStatusCode.Canceled;
                        booking.CanceledOn = DateTime.Now;
                        var bookingUpdated = context.Bookings.Attach(booking);
                        bookingUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        context.SaveChanges();
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool UpdateBookingStatus(long id, long userId, BookingStatusCode bookingStatus)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            bool result = false;
            //if (id > 0 && context.Bookings.Any(x => x.Id == id))
            //{
                var booking = context.Bookings.AsNoTracking().FirstOrDefault(x => x.Id == id && x.UserId == userId);
                booking.BookingStatusId = (int)bookingStatus;
                var bookingUpdated = context.Bookings.Attach(booking);
                bookingUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                result = true;
            //}
            return result;
        }
    }
}

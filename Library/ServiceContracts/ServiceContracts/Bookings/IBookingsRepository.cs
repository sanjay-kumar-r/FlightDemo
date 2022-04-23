using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.Bookings
{
    public interface IBookingsRepository
    {
        IEnumerable<BookingsDTOs.Bookings> GetBookings(long userId, long? id = null);

        IEnumerable<BookingsDTOs.Bookings> GetBookingsByFiltercondition(BookingsDTOs.Bookings booking);

        long BookTicket(BookingsDTOs.Bookings booking);

        bool CancelTicketByBookingIds(List<long> ids, long userId);
    }
}

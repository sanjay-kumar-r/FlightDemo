using CommonDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceContracts.Bookings;

namespace Flight.Users.Model.Utils
{
    public class BookingsRepository : IBookingsRepository
    {
        private readonly BookingsDBContext context;

        public BookingsRepository(BookingsDBContext context)
        {
            this.context = context;
        }
        
    }
}

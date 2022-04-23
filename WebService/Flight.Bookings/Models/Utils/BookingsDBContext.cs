using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Users.Model.Utils
{
    public class BookingsDBContext : DbContext
    {
        public BookingsDBContext(DbContextOptions<BookingsDBContext> options) : base(options)
        {
        }

        public DbSet<BookingsDTOs.Bookings> Bookings { get; set; }
        public DbSet<BookingsDTOs.BookingStatus> BookingStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingsDTOs.BookingStatus>()
                .HasKey(a => a.Id)
                .HasName("PrimaryKey_BookingStatusId");
            modelBuilder.Entity<BookingsDTOs.BookingStatus>()
                   .Property(t => t.Status)
                   .IsRequired();

            PopulateBookingStatus(ref modelBuilder);

            modelBuilder.Entity<BookingsDTOs.Bookings>()
                .HasKey(u => u.Id)
                .HasName("PrimaryKey_BookingId");
            modelBuilder.Entity<BookingsDTOs.Bookings>()
                    .Property(t => t.UserId)
                    .IsRequired();
            modelBuilder.Entity<BookingsDTOs.Bookings>()
                   .Property(t => t.ScheduleId)
                   .IsRequired();
            modelBuilder.Entity<BookingsDTOs.Bookings>()
                   .Property(t => t.DateBookedFor)
                   .IsRequired();
            modelBuilder.Entity<BookingsDTOs.Bookings>()
                   .Property(t => t.BookingStatusId)
                   .IsRequired();
            // modelBuilder.Entity<BookingsDTOs.Bookings>()
            // .HasRequired<BookingsDTOs.BookingStatus>(s => s.)
            // .WithMany(g => g.Students)
            // .HasForeignKey<int>(s => s.CurrentGradeId);
        }

        protected void PopulateBookingStatus(ref ModelBuilder modelBuilder)
        {
            List<BookingsDTOs.BookingStatus> accountStatus = new List<BookingsDTOs.BookingStatus>();
            foreach (var status in Enum.GetNames(typeof(BookingsDTOs.BookingStatusCode)))
            {
                accountStatus.Add(new BookingsDTOs.BookingStatus()
                {
                    Id = (int)Enum.Parse(typeof(BookingsDTOs.BookingStatusCode), status),
                    Status = status,
                    Description = BookingsDTOs.BookingStatusDescription.accountStatusDescriptions[status.ToUpper()]
                });
            }
            modelBuilder.Entity<BookingsDTOs.BookingStatus>().HasData(accountStatus);
        }
    }
}

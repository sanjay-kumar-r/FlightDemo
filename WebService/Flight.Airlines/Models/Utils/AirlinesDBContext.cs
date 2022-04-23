using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Airlines.Models.Utils
{
    public class AirlinesDBContext : DbContext
    {
        public AirlinesDBContext(DbContextOptions<AirlinesDBContext> options) : base(options)
        {
        }

        public DbSet<AirlinesDTOs.Airlines> Airlines { get; set; }
        public DbSet<AirlinesDTOs.DiscountTags> DiscountTags { get; set; }
        public DbSet<AirlinesDTOs.AirlineDiscountTagMappings> AirlineDiscountTagMappings { get; set; }
        public DbSet<AirlinesDTOs.AirlineSchedules> AirlineSchedules { get; set; }
        public DbSet<AirlinesDTOs.AirlineScheduleTracker> AirlineScheduleTracker { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //AirlineDiscountTagMappings
            modelBuilder.Entity<AirlinesDTOs.AirlineDiscountTagMappings>()
                .HasKey(x => x.Id)
                .HasName("PrimaryKey_AirDiscTagMapId");
            modelBuilder.Entity<AirlinesDTOs.AirlineDiscountTagMappings>()
                   .Property(x => x.AirlineId)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.AirlineDiscountTagMappings>()
                  .Property(x => x.DiscountTagId)
                  .IsRequired();

            //Airlines
            modelBuilder.Entity<AirlinesDTOs.Airlines>()
                .HasKey(u => u.Id)
                .HasName("PrimaryKey_AirlineId");
            modelBuilder.Entity<AirlinesDTOs.Airlines>()
                    .Property(t => t.Name)
                    .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.Airlines>()
                   .Property(t => t.AirlineCode)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.Airlines>()
                   .Property(t => t.ContactNumber)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.Airlines>()
                   .Property(t => t.TotalSeats)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.Airlines>()
                   .Property(t => t.Createdby)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.Airlines>()
                   .Property(t => t.ModifiedBy)
                   .IsRequired();

            //DiscountTags
            modelBuilder.Entity<AirlinesDTOs.DiscountTags>()
                .HasKey(u => u.Id)
                .HasName("PrimaryKey_DiscountTagId");
            modelBuilder.Entity<AirlinesDTOs.DiscountTags>()
                    .Property(t => t.Name)
                    .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.DiscountTags>()
                   .Property(t => t.DiscountCode)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.DiscountTags>()
                   .Property(t => t.Discount)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.DiscountTags>()
                   .Property(t => t.Createdby)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.DiscountTags>()
                   .Property(t => t.ModifiedBy)
                   .IsRequired();

            //AirlineScledules
            modelBuilder.Entity<AirlinesDTOs.AirlineSchedules>()
                .HasKey(u => u.Id)
                .HasName("PrimaryKey_ScheduleId");
            modelBuilder.Entity<AirlinesDTOs.AirlineSchedules>()
                    .Property(t => t.AirlineId)
                    .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.AirlineSchedules>()
                   .Property(t => t.From)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.AirlineSchedules>()
                  .Property(t => t.To)
                  .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.AirlineSchedules>()
                   .Property(t => t.Createdby)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.AirlineSchedules>()
                   .Property(t => t.ModifiedBy)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.AirlineSchedules>()
                   .Property(t => t.DepartureTime)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.AirlineSchedules>()
                   .Property(t => t.ArrivalTime)
                   .IsRequired();

            //AirlineScheduleTracker
            modelBuilder.Entity<AirlinesDTOs.AirlineScheduleTracker>()
                .HasKey(x => x.Id)
                .HasName("PrimaryKey_ScheduleTrackerId");
            modelBuilder.Entity<AirlinesDTOs.AirlineScheduleTracker>()
                   .Property(x => x.ScheduleId)
                   .IsRequired();
            modelBuilder.Entity<AirlinesDTOs.AirlineScheduleTracker>()
                   .Property(x => x.ActualDepartureDate)
                   .IsRequired();
        }
    }
}

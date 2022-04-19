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
        }
    }
}

﻿// <auto-generated />
using System;
using Flight.Users.Model.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Flight.Bookings.Migrations
{
    [DbContext(typeof(BookingsDBContext))]
    [Migration("20220424164743_bookingInitialMigration")]
    partial class bookingInitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.16")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BookingsDTOs.BookingStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("Id")
                        .HasName("PrimaryKey_BookingStatusId");

                    b.ToTable("BookingStatus");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "When airline schedule successfully booked",
                            Status = "Booked"
                        },
                        new
                        {
                            Id = 2,
                            Description = "When seats are already filled",
                            Status = "Waiting"
                        },
                        new
                        {
                            Id = 3,
                            Description = "When user cancels Booking",
                            Status = "Canceled"
                        },
                        new
                        {
                            Id = 4,
                            Description = "When user gets refunded back",
                            Status = "Refunded"
                        },
                        new
                        {
                            Id = 5,
                            Description = "When user initiates an invalid booking",
                            Status = "Invalid"
                        });
                });

            modelBuilder.Entity("BookingsDTOs.Bookings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("ActualPaidAmount")
                        .HasColumnType("float");

                    b.Property<int>("BCSeats")
                        .HasColumnType("int");

                    b.Property<int>("BookingStatusId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CanceledOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateBookedFor")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsRefunded")
                        .HasColumnType("bit");

                    b.Property<int>("NBCSeats")
                        .HasColumnType("int");

                    b.Property<string>("PNR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ScheduleId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id")
                        .HasName("PrimaryKey_BookingId");

                    b.HasIndex("BookingStatusId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("BookingsDTOs.Bookings", b =>
                {
                    b.HasOne("BookingsDTOs.BookingStatus", "BookingStatus")
                        .WithMany()
                        .HasForeignKey("BookingStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BookingStatus");
                });
#pragma warning restore 612, 618
        }
    }
}
﻿// <auto-generated />
using System;
using Flight.Airlines.Models.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Flight.Airlines.Migrations
{
    [DbContext(typeof(AirlinesDBContext))]
    [Migration("20220424171245_airlinesMigration")]
    partial class airlinesMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.16")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AirlinesDTOs.AirlineDiscountTagMappings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AirlineId")
                        .HasColumnType("bigint");

                    b.Property<long>("DiscountTagId")
                        .HasColumnType("bigint");

                    b.Property<long>("TaggedBy")
                        .HasColumnType("bigint");

                    b.HasKey("Id")
                        .HasName("PrimaryKey_AirDiscTagMapId");

                    b.HasIndex("AirlineId");

                    b.HasIndex("DiscountTagId");

                    b.ToTable("AirlineDiscountTagMappings");
                });

            modelBuilder.Entity("AirlinesDTOs.AirlineScheduleTracker", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ActualArrivalDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ActualDepartureDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("BCSeatsRemaining")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("NBCSeatsRemaining")
                        .HasColumnType("int");

                    b.Property<long>("ScheduleId")
                        .HasColumnType("bigint");

                    b.HasKey("Id")
                        .HasName("PrimaryKey_ScheduleTrackerId");

                    b.HasIndex("ScheduleId");

                    b.ToTable("AirlineScheduleTracker");
                });

            modelBuilder.Entity("AirlinesDTOs.AirlineSchedules", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AirlineId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ArrivalDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ArrivalDay")
                        .HasColumnType("int");

                    b.Property<DateTime>("ArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Createdby")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("DepartureDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DepartureDay")
                        .HasColumnType("int");

                    b.Property<DateTime>("DepartureTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRegular")
                        .HasColumnType("bit");

                    b.Property<long>("ModifiedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("Id")
                        .HasName("PrimaryKey_ScheduleId");

                    b.HasIndex("AirlineId");

                    b.ToTable("AirlineSchedules");
                });

            modelBuilder.Entity("AirlinesDTOs.Airlines", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AirlineCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("BCTicketCost")
                        .HasColumnType("float");

                    b.Property<string>("ContactAddress")
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("ContactNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Createdby")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<long>("ModifiedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<double>("NBCTicketCost")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int>("TotalBCSeats")
                        .HasColumnType("int");

                    b.Property<int>("TotalNBCSeats")
                        .HasColumnType("int");

                    b.Property<int>("TotalSeats")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PrimaryKey_AirlineId");

                    b.ToTable("Airlines");
                });

            modelBuilder.Entity("AirlinesDTOs.DiscountTags", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<long>("Createdby")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<float>("Discount")
                        .HasColumnType("real");

                    b.Property<string>("DiscountCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsByRate")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<long>("ModifiedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("Id")
                        .HasName("PrimaryKey_DiscountTagId");

                    b.ToTable("DiscountTags");
                });

            modelBuilder.Entity("AirlinesDTOs.AirlineDiscountTagMappings", b =>
                {
                    b.HasOne("AirlinesDTOs.Airlines", "Airline")
                        .WithMany()
                        .HasForeignKey("AirlineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AirlinesDTOs.DiscountTags", "DiscountTag")
                        .WithMany()
                        .HasForeignKey("DiscountTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Airline");

                    b.Navigation("DiscountTag");
                });

            modelBuilder.Entity("AirlinesDTOs.AirlineScheduleTracker", b =>
                {
                    b.HasOne("AirlinesDTOs.AirlineSchedules", "AirlineSchedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AirlineSchedule");
                });

            modelBuilder.Entity("AirlinesDTOs.AirlineSchedules", b =>
                {
                    b.HasOne("AirlinesDTOs.Airlines", "Airline")
                        .WithMany()
                        .HasForeignKey("AirlineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Airline");
                });
#pragma warning restore 612, 618
        }
    }
}
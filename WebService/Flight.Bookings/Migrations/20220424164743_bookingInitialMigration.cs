using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Flight.Bookings.Migrations
{
    public partial class bookingInitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_BookingStatusId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    DateBookedFor = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BCSeats = table.Column<int>(type: "int", nullable: false),
                    NBCSeats = table.Column<int>(type: "int", nullable: false),
                    ActualPaidAmount = table.Column<double>(type: "float", nullable: false),
                    BookingStatusId = table.Column<int>(type: "int", nullable: false),
                    PNR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CanceledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRefunded = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_BookingId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_BookingStatus_BookingStatusId",
                        column: x => x.BookingStatusId,
                        principalTable: "BookingStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BookingStatus",
                columns: new[] { "Id", "Description", "Status" },
                values: new object[,]
                {
                    { 1, "When airline schedule successfully booked", "Booked" },
                    { 2, "When seats are already filled", "Waiting" },
                    { 3, "When user cancels Booking", "Canceled" },
                    { 4, "When user gets refunded back", "Refunded" },
                    { 5, "When user initiates an invalid booking", "Invalid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingStatusId",
                table: "Bookings",
                column: "BookingStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "BookingStatus");
        }
    }
}

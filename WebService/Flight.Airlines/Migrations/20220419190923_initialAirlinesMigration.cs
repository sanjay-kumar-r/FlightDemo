using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Flight.Airlines.Migrations
{
    public partial class initialAirlinesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airlines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    AirlineCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactAddress = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TotalSeats = table.Column<int>(type: "int", nullable: false),
                    TotalBCSeats = table.Column<int>(type: "int", nullable: false),
                    TotalNBCSeats = table.Column<int>(type: "int", nullable: false),
                    BCTicketCost = table.Column<double>(type: "float", nullable: false),
                    NBCTicketCost = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Createdby = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_AirlineId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DiscountCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Discount = table.Column<float>(type: "real", nullable: false),
                    IsByRate = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Createdby = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_DiscountTagId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirlineDiscountTagMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AirlineId = table.Column<long>(type: "bigint", nullable: false),
                    DiscountTagId = table.Column<long>(type: "bigint", nullable: false),
                    TaggedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_AirDiscTagMapId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirlineDiscountTagMappings_Airlines_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "Airlines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AirlineDiscountTagMappings_DiscountTags_DiscountTagId",
                        column: x => x.DiscountTagId,
                        principalTable: "DiscountTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineDiscountTagMappings_AirlineId",
                table: "AirlineDiscountTagMappings",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_AirlineDiscountTagMappings_DiscountTagId",
                table: "AirlineDiscountTagMappings",
                column: "DiscountTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirlineDiscountTagMappings");

            migrationBuilder.DropTable(
                name: "Airlines");

            migrationBuilder.DropTable(
                name: "DiscountTags");
        }
    }
}

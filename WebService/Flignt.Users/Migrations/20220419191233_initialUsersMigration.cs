using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Flight.Users.Migrations
{
    public partial class initialUsersMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_AccStatusId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    AccountStatusId = table.Column<int>(type: "int", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_UserId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_AccountStatus_AccountStatusId",
                        column: x => x.AccountStatusId,
                        principalTable: "AccountStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AccountStatus",
                columns: new[] { "Id", "Description", "Status" },
                values: new object[,]
                {
                    { 1, "On user first time register", "Registered" },
                    { 2, "On user first time login and there after", "Active" },
                    { 3, "On user not loggedIn long time or updated by admin", "InActive" },
                    { 4, "On user invalid/wrong attempt to login or update by admin", "Blocked" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccountStatusId",
                table: "Users",
                column: "AccountStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AccountStatus");
        }
    }
}

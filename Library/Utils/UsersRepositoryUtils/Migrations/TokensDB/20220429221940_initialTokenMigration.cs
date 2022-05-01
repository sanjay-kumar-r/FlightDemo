using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UsersRepositoryUtils.Migrations.TokensDB
{
    public partial class initialTokenMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "AccountStatus",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Status = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AccountStatus", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        FirstName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            //        LastName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            //        EmailId = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
            //        Password = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
            //        AccountStatusId = table.Column<int>(type: "int", nullable: false),
            //        IsSuperAdmin = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Users_AccountStatus_AccountStatusId",
            //            column: x => x.AccountStatusId,
            //            principalTable: "AccountStatus",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInvalidated = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_UserRefreshTokensId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_UserId",
                table: "UserRefreshTokens",
                column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_AccountStatusId",
            //    table: "Users",
            //    column: "AccountStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRefreshTokens");

            //migrationBuilder.DropTable(
            //    name: "Users");

            //migrationBuilder.DropTable(
            //    name: "AccountStatus");
        }
    }
}

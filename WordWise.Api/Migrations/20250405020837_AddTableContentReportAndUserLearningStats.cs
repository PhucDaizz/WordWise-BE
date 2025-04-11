using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTableContentReportAndUserLearningStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentReport",
                columns: table => new
                {
                    ContentReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentReport", x => x.ContentReportId);
                    table.ForeignKey(
                        name: "FK_ContentReport_ExtendedIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ExtendedIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLearningStats",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false),
                    LongestStreak = table.Column<int>(type: "int", nullable: false),
                    TotalLearningHours = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    LastLearningDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLearningStats", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserLearningStats_ExtendedIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ExtendedIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "828cf79c-3fc9-40f7-8510-08779f8aaff2", "AQAAAAIAAYagAAAAEHp3s9IHqY0XvXaHz8ynXcxOz7IKZPNbuX0c5SRBhhqk6bVMk5X91b2TgXrdjBaqjw==", "4ed42858-1214-4afa-8a6f-e9aef9e7ee44" });

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "742366ea-d53c-4d50-8644-fff0de647afd");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "e544e937-29dc-4781-9268-44e1baffb45b");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "7eaf4091-b3f3-46e6-b3af-11d708f2193d");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReport_UserId",
                table: "ContentReport",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentReport");

            migrationBuilder.DropTable(
                name: "UserLearningStats");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0cbc449d-b787-4ee1-8725-b5c19b49f62c", "AQAAAAIAAYagAAAAEEUMqQdALBNZ6hxFNySuhOXX8EqBmyxYMcoK8oLpKLhxU/o4l5TAnCCIHIZUnFGjUA==", "f59f7f6a-a405-4d2b-93cf-f75247ad2da2" });

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "73947820-76cc-457b-a643-4352cf2a178f");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "6c7e9091-a8ba-4241-9316-424542db2fdf");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "1386a190-a496-4a04-a1a3-c2deb75c2605");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionTimeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentReport_ExtendedIdentityUsers_UserId",
                table: "ContentReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentReport",
                table: "ContentReport");

            migrationBuilder.DropColumn(
                name: "TotalLearningHours",
                table: "UserLearningStats");

            migrationBuilder.RenameTable(
                name: "ContentReport",
                newName: "ContentReports");

            migrationBuilder.RenameIndex(
                name: "IX_ContentReport_UserId",
                table: "ContentReports",
                newName: "IX_ContentReports_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLearningDate",
                table: "UserLearningStats",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "SessionEndTime",
                table: "UserLearningStats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SessionStartTime",
                table: "UserLearningStats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalLearningMinutes",
                table: "UserLearningStats",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentReports",
                table: "ContentReports",
                column: "ContentReportId");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "caddaf47-d247-4942-a929-de4ec8a82557", "AQAAAAIAAYagAAAAEHeD6tl2zm6fTRe0icZPWZzCijTqxhymXVbQM3sM/7ckWxwhQcxtPnIppn9Cm8vk4w==", "9e351c5b-ba24-40e3-8be7-121f33a0be37" });

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "91d13568-fd9d-4f6f-a9e1-d79c767891e8");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "b0e0e505-53aa-46be-98a1-27c22b214abc");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "edc094a8-8996-46da-b302-f9c2509ce9a0");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentReports_ExtendedIdentityUsers_UserId",
                table: "ContentReports",
                column: "UserId",
                principalTable: "ExtendedIdentityUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentReports_ExtendedIdentityUsers_UserId",
                table: "ContentReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentReports",
                table: "ContentReports");

            migrationBuilder.DropColumn(
                name: "SessionEndTime",
                table: "UserLearningStats");

            migrationBuilder.DropColumn(
                name: "SessionStartTime",
                table: "UserLearningStats");

            migrationBuilder.DropColumn(
                name: "TotalLearningMinutes",
                table: "UserLearningStats");

            migrationBuilder.RenameTable(
                name: "ContentReports",
                newName: "ContentReport");

            migrationBuilder.RenameIndex(
                name: "IX_ContentReports_UserId",
                table: "ContentReport",
                newName: "IX_ContentReport_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLearningDate",
                table: "UserLearningStats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalLearningHours",
                table: "UserLearningStats",
                type: "decimal(5,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentReport",
                table: "ContentReport",
                column: "ContentReportId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ContentReport_ExtendedIdentityUsers_UserId",
                table: "ContentReport",
                column: "UserId",
                principalTable: "ExtendedIdentityUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

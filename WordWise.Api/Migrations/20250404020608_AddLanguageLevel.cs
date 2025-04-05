using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "MultipleChoiceTests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "FlashcardSets",
                type: "int",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "MultipleChoiceTests");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "FlashcardSets");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6663aeae-a21a-4164-8f9b-057799c134e0", "AQAAAAIAAYagAAAAEEF6U5pJgekb9bQk/EGPTFy/ubeY7RITtHf0vTiqytcCKh3jI+yvqDBPxlmCgLEU3w==", "9049049c-3482-47b5-b81a-2a5b887d1576" });

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "94658d41-19a9-4ea6-8631-cc4f29da6aa1");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "bf97ff77-7686-4e4a-8f14-2701cbf5ed6b");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "b2d1aafe-cfe7-44e5-bb0c-7b03f01ef621");
        }
    }
}

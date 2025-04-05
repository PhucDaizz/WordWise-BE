using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLearnerCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LearnerCount",
                table: "MultipleChoiceTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LearnerCount",
                table: "FlashcardSets",
                type: "int",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearnerCount",
                table: "MultipleChoiceTests");

            migrationBuilder.DropColumn(
                name: "LearnerCount",
                table: "FlashcardSets");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ef2c5f5f-e7ad-46d3-82f5-9e26c19ffae2", "AQAAAAIAAYagAAAAEPIRmnMvm9iVyJHq85Q/+jWUzMRBPQE5sGu7zckZrBz1f3nIpRbJ68OEMDakS6xGyA==", "23a67f91-f667-4291-b0e8-00afbf9a9cce" });

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "8fa8fd60-da78-4f78-a104-2419ade10a9f");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "6efe2bb7-3039-4593-85d5-e906b2e736d1");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "31529ce2-077a-4ef7-bd44-60b57640bb87");
        }
    }
}

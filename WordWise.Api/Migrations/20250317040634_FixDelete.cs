using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardSets_FlashcardSetId",
                table: "Flashcards");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1a55ce92-7bbe-4488-a590-7a7db4d6e565", "AQAAAAIAAYagAAAAEAD8JpP8+JkvaeeLX4ojOzm9BBTF+g/MogtDTxQFaEY/AHfWgKAypiJYO5EwFq3X6g==", "786f063f-0ded-4f41-90c8-4c73f9bbc410" });

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "53dc8c65-6b10-467c-9b38-abe950de56b9");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "1ca8d0bc-f893-45fb-a274-358a5aa987ae");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "02ac7313-f8c4-4392-bfea-7a10a39ad3c3");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardSets_FlashcardSetId",
                table: "Flashcards",
                column: "FlashcardSetId",
                principalTable: "FlashcardSets",
                principalColumn: "FlashcardSetId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardSets_FlashcardSetId",
                table: "Flashcards");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c48b7b54-3bd4-483a-9572-2d22321a9237", "AQAAAAIAAYagAAAAEICWsMhv0vtDjWjM5tQcq/8VX4fVH52a2zmPEF1loW0g3sJszs+eeDkr7ap9CaVy3A==", "d456afd1-67e9-48dc-80f8-f7fc4a57781b" });

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "0ec19d70-a0d3-46ee-a0aa-2502443be36c");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "c6cdf17f-ef27-4966-8504-ac0f139be323");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "b4365062-08e1-41ee-9b00-c8a7815f6ab6");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardSets_FlashcardSetId",
                table: "Flashcards",
                column: "FlashcardSetId",
                principalTable: "FlashcardSets",
                principalColumn: "FlashcardSetId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

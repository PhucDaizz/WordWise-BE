using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddComlumnCurrentQuestionIndexAndLastAnsForTableRoomParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentQuestionIndex",
                table: "RoomParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAnswerSubmittedAt",
                table: "RoomParticipants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0a498fcd-9de6-412a-aec7-bfe7431d22f4", "AQAAAAIAAYagAAAAEAM3AwJp3Thau0PKU8seUqIfMiQtYzERiR6B2NUqpDt2dxpCeOXjJSIdlqoEp4WrAg==", "0e876eba-7875-4eeb-8f44-adabe39c4e76" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "cbcb7f0a-59e4-4da3-a811-d7c859e8f2f5");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "682f037f-1ded-4274-ac49-267cd877b411");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "5ea3d794-4c6d-47af-95a8-63a200f9d49c");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentQuestionIndex",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "LastAnswerSubmittedAt",
                table: "RoomParticipants");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d8b1ecad-39ff-4d40-85fd-6bd3c3b09468", "AQAAAAIAAYagAAAAEH3vRJb/gl1AKKBxaHTnUVJnrNCXAjIe97LeWWapWFY4b3yk/2oWo3wENS7eIj4nog==", "bd260fca-419a-4285-8290-f472e99fcf2d" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "9475f3ed-9fb4-4cbf-9d03-45579bad3c24");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "2cfde6ee-584c-4d7c-9be1-ff6990c9380d");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "39bb64c3-ea8c-4e1d-81ce-0aa87616c768");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRommOnlineFlashcard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FlashcardSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: true),
                    CurrentQuestionIndex = table.Column<int>(type: "int", nullable: false),
                    ShowLeaderboardRealtime = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_Rooms_ExtendedIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ExtendedIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rooms_FlashcardSets_FlashcardSetId",
                        column: x => x.FlashcardSetId,
                        principalTable: "FlashcardSets",
                        principalColumn: "FlashcardSetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomParticipants",
                columns: table => new
                {
                    RoomParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomParticipants", x => x.RoomParticipantId);
                    table.ForeignKey(
                        name: "FK_RoomParticipants_ExtendedIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ExtendedIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomParticipants_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentFlashcardAttempts",
                columns: table => new
                {
                    AttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlashcardId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeTakenMs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFlashcardAttempts", x => x.AttemptId);
                    table.ForeignKey(
                        name: "FK_StudentFlashcardAttempts_Flashcards_FlashcardId",
                        column: x => x.FlashcardId,
                        principalTable: "Flashcards",
                        principalColumn: "FlashcardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentFlashcardAttempts_RoomParticipants_RoomParticipantId",
                        column: x => x.RoomParticipantId,
                        principalTable: "RoomParticipants",
                        principalColumn: "RoomParticipantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentFlashcardAttempts_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_RoomId",
                table: "RoomParticipants",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_UserId",
                table: "RoomParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_FlashcardSetId",
                table: "Rooms",
                column: "FlashcardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_UserId",
                table: "Rooms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFlashcardAttempts_FlashcardId",
                table: "StudentFlashcardAttempts",
                column: "FlashcardId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFlashcardAttempts_RoomId",
                table: "StudentFlashcardAttempts",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFlashcardAttempts_RoomParticipantId",
                table: "StudentFlashcardAttempts",
                column: "RoomParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentFlashcardAttempts");

            migrationBuilder.DropTable(
                name: "RoomParticipants");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "17b9b273-a430-450f-83ab-341de593a4a4", "AQAAAAIAAYagAAAAENmb96dlY8KLl0LnTQpDVyZhsAgzmftLAooeVGk7mz0k8Y3i9rgoJ78giZ7mZiMbwA==", "7f71c827-2067-4cab-b0a8-91942be65910" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "4000fd53-bc67-4afd-870c-f189535b12b7");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "7fe356ea-3dee-474d-9ff0-c46118820f2d");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "afbb984b-5fd5-4088-a863-aa69417d1e4d");
        }
    }
}

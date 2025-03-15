using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExtendedIdentityUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtendedIdentityUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlashcardSets",
                columns: table => new
                {
                    FlashcardSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LearningLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NativeLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardSets", x => x.FlashcardSetId);
                    table.ForeignKey(
                        name: "FK_FlashcardSets_ExtendedIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ExtendedIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MultipleChoiceTests",
                columns: table => new
                {
                    MultipleChoiceTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LearningLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NativeLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleChoiceTests", x => x.MultipleChoiceTestId);
                    table.ForeignKey(
                        name: "FK_MultipleChoiceTests_ExtendedIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ExtendedIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WritingExercises",
                columns: table => new
                {
                    WritingExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AIFeedback = table.Column<int>(type: "int", nullable: false),
                    LearningLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NativeLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingExercises", x => x.WritingExerciseId);
                    table.ForeignKey(
                        name: "FK_WritingExercises_ExtendedIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ExtendedIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlashcardReviews",
                columns: table => new
                {
                    FlashcardReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FlashcardSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardReviews", x => x.FlashcardReviewId);
                    table.ForeignKey(
                        name: "FK_FlashcardReviews_ExtendedIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ExtendedIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlashcardReviews_FlashcardSets_FlashcardSetId",
                        column: x => x.FlashcardSetId,
                        principalTable: "FlashcardSets",
                        principalColumn: "FlashcardSetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Flashcards",
                columns: table => new
                {
                    FlashcardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Term = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Definition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Example = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlashcardSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flashcards", x => x.FlashcardId);
                    table.ForeignKey(
                        name: "FK_Flashcards_FlashcardSets_FlashcardSetId",
                        column: x => x.FlashcardSetId,
                        principalTable: "FlashcardSets",
                        principalColumn: "FlashcardSetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MultipleChoiceTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer_a = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Answer_b = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Answer_c = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Answer_d = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CorrectAnswer = table.Column<int>(type: "int", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_MultipleChoiceTests_MultipleChoiceTestId",
                        column: x => x.MultipleChoiceTestId,
                        principalTable: "MultipleChoiceTests",
                        principalColumn: "MultipleChoiceTestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "477d3788-e4b3-4f3d-8dbd-aaead19b78ab", null, "Admin", "ADMIN" },
                    { "8bc05967-a01b-424c-a760-475af79c738f", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardReviews_FlashcardSetId",
                table: "FlashcardReviews",
                column: "FlashcardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardReviews_UserId",
                table: "FlashcardReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_FlashcardSetId",
                table: "Flashcards",
                column: "FlashcardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardSets_UserId",
                table: "FlashcardSets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleChoiceTests_UserId",
                table: "MultipleChoiceTests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_MultipleChoiceTestId",
                table: "Questions",
                column: "MultipleChoiceTestId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingExercises_UserId",
                table: "WritingExercises",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlashcardReviews");

            migrationBuilder.DropTable(
                name: "Flashcards");

            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "WritingExercises");

            migrationBuilder.DropTable(
                name: "FlashcardSets");

            migrationBuilder.DropTable(
                name: "MultipleChoiceTests");

            migrationBuilder.DropTable(
                name: "ExtendedIdentityUsers");
        }
    }
}

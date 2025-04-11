using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserRole<string>",
                table: "IdentityUserRole<string>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityRole",
                table: "IdentityRole");

            migrationBuilder.RenameTable(
                name: "IdentityUserRole<string>",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "IdentityRole",
                newName: "Roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "IdentityUserRole<string>");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "IdentityRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserRole<string>",
                table: "IdentityUserRole<string>",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityRole",
                table: "IdentityRole",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "ExtendedIdentityUsers",
                keyColumn: "Id",
                keyValue: "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "212569da-f0fd-449f-a761-758d195e471b", "AQAAAAIAAYagAAAAEPKyoFNOgaYJXhMcfCl/1UvfGcEW2qEB1Ym0u6D5R4dnVgDVlYRTxkuyDTyP8NIwPw==", "febd5101-20b9-4d7e-98a9-40fe1a86864b" });

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                column: "ConcurrencyStamp",
                value: "e4b4258c-a3f6-4f03-ab0e-8a822ee5f431");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8bc05967-a01b-424c-a760-475af79c738f",
                column: "ConcurrencyStamp",
                value: "2154fdec-691d-436b-b6ba-c7f82be14e99");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                column: "ConcurrencyStamp",
                value: "5dd2ea47-7aca-4daf-bc3b-78984a06db13");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserControl.Migrations
{
    /// <inheritdoc />
    public partial class AdminSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5861c623-9585-42cb-a37c-5be2e9f4a528", "2169a735-d5d1-4157-a048-42ae8b125dec", "admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d6e972db-6e0a-4b5a-8bde-4d34962d1706", 0, "8c0f716f-1069-4559-9028-fea7bc205d8f", null, false, false, null, null, "ADMIN", "AQAAAAEAACcQAAAAENhlyHGL/3s+iuG3sBNOLSaQ2Fru8GuJhHYcKK+k0VfDEx/C1Uph04KlCHcijIB0Sg==", null, false, "0db42630-8a85-4bf6-92c9-4d88f87727b2", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5861c623-9585-42cb-a37c-5be2e9f4a528", "d6e972db-6e0a-4b5a-8bde-4d34962d1706" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5861c623-9585-42cb-a37c-5be2e9f4a528", "d6e972db-6e0a-4b5a-8bde-4d34962d1706" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5861c623-9585-42cb-a37c-5be2e9f4a528");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d6e972db-6e0a-4b5a-8bde-4d34962d1706");
        }
    }
}

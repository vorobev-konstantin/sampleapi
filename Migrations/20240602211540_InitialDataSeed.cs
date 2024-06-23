using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sampleapi.Migrations
{
    /// <inheritdoc />
    public partial class InitialDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Phone", "UserName" },
                values: new object[,]
                {
                    { 1L, "peskovv@ya.ru", "Vladislav", "Peskov", null, "peskovv" },
                    { 2L, "feforovr@ya.ru", "Roman", "Fedorov", "+73450934543", "fedorovr" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L);
        }
    }
}

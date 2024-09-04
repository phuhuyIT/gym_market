using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoleMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "25501994-44dd-44b8-bb7d-1b2af376f1be", "1", "Admin", "Admin" },
                    { "32b89678-1f5d-43c8-8dbd-4251902bdfa4", "2", "Trainer", "Trainer" },
                    { "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7", "3", "Member", "Member" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "25501994-44dd-44b8-bb7d-1b2af376f1be");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "32b89678-1f5d-43c8-8dbd-4251902bdfa4");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7");
        }
    }
}

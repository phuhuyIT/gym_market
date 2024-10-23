using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "25501994-44dd-44b8-bb7d-1b2af376f1be", "1", "Admin", "Admin" },
                    { "32b89678-1f5d-43c8-8dbd-4251902bdfa4", "2", "Trainer", "Trainer" },
                    { "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7", "3", "Member", "Member" }
                });

            migrationBuilder.InsertData(
                table: "Trainers",
                columns: new[] { "Trainer_ID", "Bio", "Certification", "Email", "Experience", "Name", "Password", "Profile_Picture", "Rating", "Updated_At", "UserId" },
                values: new object[,]
                {
                    { "TR001", null, "YogaStrong", null, null, "John Doe", null, null, 4.5m, null, null },
                    { "TR002", null, "GymStrong", null, null, "Jane Smith", null, null, 4.8m, null, null }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Course_ID", "Category", "Description", "Duration", "End_Date", "Max_Participants", "Price", "Rating", "Start_Date", "Title", "Trainer_ID", "Type" },
                values: new object[,]
                {
                    { "C001", "Yoga", "A beginner-level yoga course.", 30, new DateTime(2024, 12, 1, 22, 0, 45, 328, DateTimeKind.Local).AddTicks(4573), 20, 100m, 4.6m, new DateTime(2024, 11, 1, 22, 0, 45, 328, DateTimeKind.Local).AddTicks(4544), "Beginner Yoga", "TR001", null },
                    { "C002", "Fitness", "An advanced course for fitness enthusiasts.", 30, new DateTime(2024, 12, 6, 22, 0, 45, 328, DateTimeKind.Local).AddTicks(4586), 25, 150m, 4.9m, new DateTime(2024, 11, 6, 22, 0, 45, 328, DateTimeKind.Local).AddTicks(4585), "Advanced Fitness", "TR002", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25501994-44dd-44b8-bb7d-1b2af376f1be");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "32b89678-1f5d-43c8-8dbd-4251902bdfa4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Course_ID",
                keyValue: "C001");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Course_ID",
                keyValue: "C002");

            migrationBuilder.DeleteData(
                table: "Trainers",
                keyColumn: "Trainer_ID",
                keyValue: "TR001");

            migrationBuilder.DeleteData(
                table: "Trainers",
                keyColumn: "Trainer_ID",
                keyValue: "TR002");
        }
    }
}

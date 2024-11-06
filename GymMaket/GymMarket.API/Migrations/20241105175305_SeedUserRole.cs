using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "Avatar", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Status", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "8e445865-a24d-4543-a6c6-9443d048cdb9", 0, null, null, "1b1b1215-a28f-4f6c-98c2-23d9f485735b", "admin@localhost.com", true, null, false, null, "ADMIN@LOCALHOST.COM", "ADMIN@LOCALHOST.COM", "AQAAAAIAAYagAAAAEPulgDbVvfqyQLuZI0DjTlMI0BfXChnsnx9JUHK+cf1uaIRnAkvzJ1tj4fBzLViRyw==", null, false, "86e1c8b8-1b79-4857-815e-341f2869f3e2", null, false, "admin@localhost.com" },
                    { "9e224968-33e4-4652-b7b7-8574d048cdb9", 0, null, null, "b9d662e2-ddf5-4f6c-a282-5265c7a99ee4", "user@localhost.com", true, null, false, null, "USER@LOCALHOST.COM", "USER@LOCALHOST.COM", "AQAAAAIAAYagAAAAENUOypATb2FoCZo1vWE144QMg4RrFOHNopjaTJmVMgN7ynrQKxKY2Rpt3eaJrSnSPA==", null, false, "5d0a00d3-3f30-472d-9ce4-4b7749e4e874", null, false, "user@localhost.com" }
                });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Course_ID",
                keyValue: "C001",
                columns: new[] { "End_Date", "Start_Date" },
                values: new object[] { new DateTime(2024, 12, 16, 0, 53, 4, 475, DateTimeKind.Local).AddTicks(5907), new DateTime(2024, 11, 16, 0, 53, 4, 475, DateTimeKind.Local).AddTicks(5880) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Course_ID",
                keyValue: "C002",
                columns: new[] { "End_Date", "Start_Date" },
                values: new object[] { new DateTime(2024, 12, 21, 0, 53, 4, 475, DateTimeKind.Local).AddTicks(5912), new DateTime(2024, 11, 21, 0, 53, 4, 475, DateTimeKind.Local).AddTicks(5911) });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "25501994-44dd-44b8-bb7d-1b2af376f1be", "8e445865-a24d-4543-a6c6-9443d048cdb9" },
                    { "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7", "9e224968-33e4-4652-b7b7-8574d048cdb9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "25501994-44dd-44b8-bb7d-1b2af376f1be", "8e445865-a24d-4543-a6c6-9443d048cdb9" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7", "9e224968-33e4-4652-b7b7-8574d048cdb9" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Course_ID",
                keyValue: "C001",
                columns: new[] { "End_Date", "Start_Date" },
                values: new object[] { new DateTime(2024, 12, 1, 22, 0, 45, 328, DateTimeKind.Local).AddTicks(4573), new DateTime(2024, 11, 1, 22, 0, 45, 328, DateTimeKind.Local).AddTicks(4544) });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Course_ID",
                keyValue: "C002",
                columns: new[] { "End_Date", "Start_Date" },
                values: new object[] { new DateTime(2024, 12, 6, 22, 0, 45, 328, DateTimeKind.Local).AddTicks(4586), new DateTime(2024, 11, 6, 22, 0, 45, 328, DateTimeKind.Local).AddTicks(4585) });
        }
    }
}

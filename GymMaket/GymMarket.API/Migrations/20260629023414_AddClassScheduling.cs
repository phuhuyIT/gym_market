using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClassScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gym_Class_Sessions",
                columns: table => new
                {
                    Class_Session_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Trainer_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Starts_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Ends_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Scheduled"),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gym_Class_Sessions", x => x.Class_Session_ID);
                    table.ForeignKey(
                        name: "FK_Gym_Class_Sessions_Trainer",
                        column: x => x.Trainer_ID,
                        principalTable: "Trainers",
                        principalColumn: "Trainer_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Class_Bookings",
                columns: table => new
                {
                    Booking_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Class_Session_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Booked"),
                    Booked_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Cancelled_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Attendance_Marked_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class_Bookings", x => x.Booking_ID);
                    table.ForeignKey(
                        name: "FK_Class_Bookings_Class_Session",
                        column: x => x.Class_Session_ID,
                        principalTable: "Gym_Class_Sessions",
                        principalColumn: "Class_Session_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Class_Bookings_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Class_Bookings_Class_Session_ID",
                table: "Class_Bookings",
                column: "Class_Session_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Class_Bookings_Class_Session_ID_Status",
                table: "Class_Bookings",
                columns: new[] { "Class_Session_ID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Class_Bookings_Class_Session_ID_Student_ID",
                table: "Class_Bookings",
                columns: new[] { "Class_Session_ID", "Student_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_Class_Bookings_Status",
                table: "Class_Bookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Class_Bookings_Student_ID",
                table: "Class_Bookings",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Gym_Class_Sessions_Starts_At",
                table: "Gym_Class_Sessions",
                column: "Starts_At");

            migrationBuilder.CreateIndex(
                name: "IX_Gym_Class_Sessions_Status",
                table: "Gym_Class_Sessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Gym_Class_Sessions_Status_Starts_At",
                table: "Gym_Class_Sessions",
                columns: new[] { "Status", "Starts_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Gym_Class_Sessions_Trainer_ID",
                table: "Gym_Class_Sessions",
                column: "Trainer_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Class_Bookings");

            migrationBuilder.DropTable(
                name: "Gym_Class_Sessions");
        }
    }
}

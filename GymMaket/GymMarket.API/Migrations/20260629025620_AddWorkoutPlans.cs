using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workout_Plans",
                columns: table => new
                {
                    Workout_Plan_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Trainer_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Difficulty = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Duration_Weeks = table.Column<int>(type: "int", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workout_Plans", x => x.Workout_Plan_ID);
                    table.ForeignKey(
                        name: "FK_Workout_Plans_Trainer",
                        column: x => x.Trainer_ID,
                        principalTable: "Trainers",
                        principalColumn: "Trainer_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Student_Workout_Assignments",
                columns: table => new
                {
                    Assignment_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Workout_Plan_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Trainer_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Active"),
                    Starts_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Ends_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Completed_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Cancelled_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student_Workout_Assignments", x => x.Assignment_ID);
                    table.ForeignKey(
                        name: "FK_Student_Workout_Assignments_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Student_Workout_Assignments_Trainer",
                        column: x => x.Trainer_ID,
                        principalTable: "Trainers",
                        principalColumn: "Trainer_ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Student_Workout_Assignments_Workout_Plan",
                        column: x => x.Workout_Plan_ID,
                        principalTable: "Workout_Plans",
                        principalColumn: "Workout_Plan_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workout_Exercises",
                columns: table => new
                {
                    Exercise_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Workout_Plan_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Week_Number = table.Column<int>(type: "int", nullable: false),
                    Day_Number = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Sets = table.Column<int>(type: "int", nullable: false),
                    Reps = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Rest_Seconds = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workout_Exercises", x => x.Exercise_ID);
                    table.ForeignKey(
                        name: "FK_Workout_Exercises_Workout_Plan",
                        column: x => x.Workout_Plan_ID,
                        principalTable: "Workout_Plans",
                        principalColumn: "Workout_Plan_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workout_Exercise_Completions",
                columns: table => new
                {
                    Completion_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Assignment_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Exercise_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Completed_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workout_Exercise_Completions", x => x.Completion_ID);
                    table.ForeignKey(
                        name: "FK_Workout_Exercise_Completions_Assignment",
                        column: x => x.Assignment_ID,
                        principalTable: "Student_Workout_Assignments",
                        principalColumn: "Assignment_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workout_Exercise_Completions_Exercise",
                        column: x => x.Exercise_ID,
                        principalTable: "Workout_Exercises",
                        principalColumn: "Exercise_ID");
                    table.ForeignKey(
                        name: "FK_Workout_Exercise_Completions_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_Workout_Assignments_Status",
                table: "Student_Workout_Assignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Workout_Assignments_Student_ID",
                table: "Student_Workout_Assignments",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Workout_Assignments_Student_ID_Status",
                table: "Student_Workout_Assignments",
                columns: new[] { "Student_ID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Student_Workout_Assignments_Trainer_ID",
                table: "Student_Workout_Assignments",
                column: "Trainer_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Workout_Assignments_Trainer_ID_Status",
                table: "Student_Workout_Assignments",
                columns: new[] { "Trainer_ID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Student_Workout_Assignments_Workout_Plan_ID",
                table: "Student_Workout_Assignments",
                column: "Workout_Plan_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Exercise_Completions_Assignment_ID",
                table: "Workout_Exercise_Completions",
                column: "Assignment_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Exercise_Completions_Assignment_ID_Exercise_ID",
                table: "Workout_Exercise_Completions",
                columns: new[] { "Assignment_ID", "Exercise_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Exercise_Completions_Exercise_ID",
                table: "Workout_Exercise_Completions",
                column: "Exercise_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Exercise_Completions_Student_ID",
                table: "Workout_Exercise_Completions",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Exercises_Workout_Plan_ID",
                table: "Workout_Exercises",
                column: "Workout_Plan_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Exercises_Workout_Plan_ID_Week_Number_Day_Number_Order",
                table: "Workout_Exercises",
                columns: new[] { "Workout_Plan_ID", "Week_Number", "Day_Number", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Plans_Is_Active",
                table: "Workout_Plans",
                column: "Is_Active");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Plans_Trainer_ID",
                table: "Workout_Plans",
                column: "Trainer_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Plans_Trainer_ID_Is_Active",
                table: "Workout_Plans",
                columns: new[] { "Trainer_ID", "Is_Active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Workout_Exercise_Completions");

            migrationBuilder.DropTable(
                name: "Student_Workout_Assignments");

            migrationBuilder.DropTable(
                name: "Workout_Exercises");

            migrationBuilder.DropTable(
                name: "Workout_Plans");
        }
    }
}

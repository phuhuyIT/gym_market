using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course_Assignments",
                columns: table => new
                {
                    Assignment_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Grade_Category_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Points_Possible = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Due_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Submission_Type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Text"),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Draft"),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Assignments", x => x.Assignment_ID);
                    table.ForeignKey(
                        name: "FK_Course_Assignments_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Course_Assignments_Grade_Category",
                        column: x => x.Grade_Category_ID,
                        principalTable: "Grade_Categories",
                        principalColumn: "Category_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Assignment_Submissions",
                columns: table => new
                {
                    Submission_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Assignment_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Text_Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attachment_Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Score = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Score_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Submitted"),
                    Feedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Submitted_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Graded_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment_Submissions", x => x.Submission_ID);
                    table.ForeignKey(
                        name: "FK_Assignment_Submissions_Assignment",
                        column: x => x.Assignment_ID,
                        principalTable: "Course_Assignments",
                        principalColumn: "Assignment_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignment_Submissions_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Submissions_Assignment_ID",
                table: "Assignment_Submissions",
                column: "Assignment_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Submissions_Assignment_ID_Student_ID",
                table: "Assignment_Submissions",
                columns: new[] { "Assignment_ID", "Student_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Submissions_Status",
                table: "Assignment_Submissions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Submissions_Student_ID",
                table: "Assignment_Submissions",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Assignments_Course_ID",
                table: "Course_Assignments",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Assignments_Course_ID_Status_Due_At",
                table: "Course_Assignments",
                columns: new[] { "Course_ID", "Status", "Due_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Assignments_Due_At",
                table: "Course_Assignments",
                column: "Due_At");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Assignments_Grade_Category_ID",
                table: "Course_Assignments",
                column: "Grade_Category_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Assignments_Status",
                table: "Course_Assignments",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignment_Submissions");

            migrationBuilder.DropTable(
                name: "Course_Assignments");
        }
    }
}

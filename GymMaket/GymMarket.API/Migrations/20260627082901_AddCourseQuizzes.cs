using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseQuizzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course_Quizzes",
                columns: table => new
                {
                    Quiz_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Passing_Score_Percent = table.Column<int>(type: "int", nullable: false),
                    Is_Published = table.Column<bool>(type: "bit", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Quizzes", x => x.Quiz_ID);
                    table.ForeignKey(
                        name: "FK_Course_Quizzes_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quiz_Attempts",
                columns: table => new
                {
                    Attempt_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Quiz_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Total_Points = table.Column<int>(type: "int", nullable: false),
                    Score_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false),
                    Submitted_At = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz_Attempts", x => x.Attempt_ID);
                    table.ForeignKey(
                        name: "FK_Quiz_Attempts_Course_Quiz",
                        column: x => x.Quiz_ID,
                        principalTable: "Course_Quizzes",
                        principalColumn: "Quiz_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quiz_Attempts_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quiz_Questions",
                columns: table => new
                {
                    Question_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Quiz_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz_Questions", x => x.Question_ID);
                    table.ForeignKey(
                        name: "FK_Quiz_Questions_Course_Quiz",
                        column: x => x.Quiz_ID,
                        principalTable: "Course_Quizzes",
                        principalColumn: "Quiz_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quiz_Options",
                columns: table => new
                {
                    Option_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Question_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Is_Correct = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz_Options", x => x.Option_ID);
                    table.ForeignKey(
                        name: "FK_Quiz_Options_Quiz_Question",
                        column: x => x.Question_ID,
                        principalTable: "Quiz_Questions",
                        principalColumn: "Question_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quiz_Attempt_Answers",
                columns: table => new
                {
                    Attempt_Answer_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Attempt_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Question_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Selected_Option_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Is_Correct = table.Column<bool>(type: "bit", nullable: false),
                    Points_Awarded = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz_Attempt_Answers", x => x.Attempt_Answer_ID);
                    table.ForeignKey(
                        name: "FK_Quiz_Attempt_Answers_Quiz_Attempt",
                        column: x => x.Attempt_ID,
                        principalTable: "Quiz_Attempts",
                        principalColumn: "Attempt_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quiz_Attempt_Answers_Quiz_Option",
                        column: x => x.Selected_Option_ID,
                        principalTable: "Quiz_Options",
                        principalColumn: "Option_ID");
                    table.ForeignKey(
                        name: "FK_Quiz_Attempt_Answers_Quiz_Question",
                        column: x => x.Question_ID,
                        principalTable: "Quiz_Questions",
                        principalColumn: "Question_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Quizzes_Course_ID",
                table: "Course_Quizzes",
                column: "Course_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Attempt_Answers_Attempt_ID",
                table: "Quiz_Attempt_Answers",
                column: "Attempt_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Attempt_Answers_Question_ID",
                table: "Quiz_Attempt_Answers",
                column: "Question_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Attempt_Answers_Selected_Option_ID",
                table: "Quiz_Attempt_Answers",
                column: "Selected_Option_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Attempts_Quiz_ID",
                table: "Quiz_Attempts",
                column: "Quiz_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Attempts_Quiz_ID_Student_ID_Submitted_At",
                table: "Quiz_Attempts",
                columns: new[] { "Quiz_ID", "Student_ID", "Submitted_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Attempts_Student_ID",
                table: "Quiz_Attempts",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Options_Question_ID",
                table: "Quiz_Options",
                column: "Question_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Questions_Quiz_ID",
                table: "Quiz_Questions",
                column: "Quiz_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Questions_Quiz_ID_Order",
                table: "Quiz_Questions",
                columns: new[] { "Quiz_ID", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quiz_Attempt_Answers");

            migrationBuilder.DropTable(
                name: "Quiz_Attempts");

            migrationBuilder.DropTable(
                name: "Quiz_Options");

            migrationBuilder.DropTable(
                name: "Quiz_Questions");

            migrationBuilder.DropTable(
                name: "Course_Quizzes");
        }
    }
}

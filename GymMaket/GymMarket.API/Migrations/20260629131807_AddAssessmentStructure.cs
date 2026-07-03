using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAssessmentStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_Quizzes_Course_ID",
                table: "Course_Quizzes");

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "Quiz_Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question_Type",
                table: "Quiz_Questions",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "SingleChoice");

            migrationBuilder.AddColumn<bool>(
                name: "Requires_Manual_Grading",
                table: "Quiz_Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Attempt_Number",
                table: "Quiz_Attempts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "Quiz_Attempts",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Graded_At",
                table: "Quiz_Attempts",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Requires_Manual_Grading",
                table: "Quiz_Attempts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Started_At",
                table: "Quiz_Attempts",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Quiz_Attempts",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "Submitted");

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "Quiz_Attempt_Answers",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Selected_Option_IDs",
                table: "Quiz_Attempt_Answers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text_Answer",
                table: "Quiz_Attempt_Answers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Available_From",
                table: "Course_Quizzes",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Available_Until",
                table: "Course_Quizzes",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Course_Quizzes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lecture_ID",
                table: "Course_Quizzes",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Max_Attempts",
                table: "Course_Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Module_ID",
                table: "Course_Quizzes",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope_Type",
                table: "Course_Quizzes",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "Course");

            migrationBuilder.AddColumn<bool>(
                name: "Show_Correct_Answers",
                table: "Course_Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Shuffle_Questions",
                table: "Course_Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Time_Limit_Minutes",
                table: "Course_Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_Quizzes_Course_ID",
                table: "Course_Quizzes",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Quizzes_Course_ID_Scope_Type",
                table: "Course_Quizzes",
                columns: new[] { "Course_ID", "Scope_Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Quizzes_Lecture_ID",
                table: "Course_Quizzes",
                column: "Lecture_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Quizzes_Module_ID",
                table: "Course_Quizzes",
                column: "Module_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Quizzes_Course_Module",
                table: "Course_Quizzes",
                column: "Module_ID",
                principalTable: "Course_Modules",
                principalColumn: "Module_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Quizzes_Lecture",
                table: "Course_Quizzes",
                column: "Lecture_ID",
                principalTable: "Lectures",
                principalColumn: "Lecture_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Quizzes_Course_Module",
                table: "Course_Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_Quizzes_Lecture",
                table: "Course_Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Course_Quizzes_Course_ID",
                table: "Course_Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Course_Quizzes_Course_ID_Scope_Type",
                table: "Course_Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Course_Quizzes_Lecture_ID",
                table: "Course_Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Course_Quizzes_Module_ID",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "Quiz_Questions");

            migrationBuilder.DropColumn(
                name: "Question_Type",
                table: "Quiz_Questions");

            migrationBuilder.DropColumn(
                name: "Requires_Manual_Grading",
                table: "Quiz_Questions");

            migrationBuilder.DropColumn(
                name: "Attempt_Number",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Graded_At",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Requires_Manual_Grading",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Started_At",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "Quiz_Attempt_Answers");

            migrationBuilder.DropColumn(
                name: "Selected_Option_IDs",
                table: "Quiz_Attempt_Answers");

            migrationBuilder.DropColumn(
                name: "Text_Answer",
                table: "Quiz_Attempt_Answers");

            migrationBuilder.DropColumn(
                name: "Available_From",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Available_Until",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Lecture_ID",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Max_Attempts",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Module_ID",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Scope_Type",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Show_Correct_Answers",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Shuffle_Questions",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Time_Limit_Minutes",
                table: "Course_Quizzes");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Quizzes_Course_ID",
                table: "Course_Quizzes",
                column: "Course_ID",
                unique: true);
        }
    }
}

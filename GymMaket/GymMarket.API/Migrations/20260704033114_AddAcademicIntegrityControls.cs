using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicIntegrityControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Question_Bank",
                table: "Quiz_Questions",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Browser_Fingerprint",
                table: "Quiz_Attempts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Focus_Lost_Count",
                table: "Quiz_Attempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Fullscreen_Exit_Count",
                table: "Quiz_Attempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Honor_Code_Accepted",
                table: "Quiz_Attempts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Ip_Address",
                table: "Quiz_Attempts",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Paste_Event_Count",
                table: "Quiz_Attempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Proctoring_Event_Count",
                table: "Quiz_Attempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Proctoring_Flags",
                table: "Quiz_Attempts",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Proctoring_Review_Required",
                table: "Quiz_Attempts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Question_Order_Snapshot",
                table: "Quiz_Attempts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Suspicious_Activity_Score",
                table: "Quiz_Attempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Require_Honor_Code",
                table: "Course_Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Shuffle_Options",
                table: "Course_Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Track_Proctoring_Signals",
                table: "Course_Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Similarity_Checked_At",
                table: "Assignment_Submissions",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Similarity_Flags",
                table: "Assignment_Submissions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Similarity_Matched_Student_Name",
                table: "Assignment_Submissions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Similarity_Matched_Submission_ID",
                table: "Assignment_Submissions",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Similarity_Score_Percent",
                table: "Assignment_Submissions",
                type: "decimal(5,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question_Bank",
                table: "Quiz_Questions");

            migrationBuilder.DropColumn(
                name: "Browser_Fingerprint",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Focus_Lost_Count",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Fullscreen_Exit_Count",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Honor_Code_Accepted",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Ip_Address",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Paste_Event_Count",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Proctoring_Event_Count",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Proctoring_Flags",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Proctoring_Review_Required",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Question_Order_Snapshot",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Suspicious_Activity_Score",
                table: "Quiz_Attempts");

            migrationBuilder.DropColumn(
                name: "Require_Honor_Code",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Shuffle_Options",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Track_Proctoring_Signals",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Similarity_Checked_At",
                table: "Assignment_Submissions");

            migrationBuilder.DropColumn(
                name: "Similarity_Flags",
                table: "Assignment_Submissions");

            migrationBuilder.DropColumn(
                name: "Similarity_Matched_Student_Name",
                table: "Assignment_Submissions");

            migrationBuilder.DropColumn(
                name: "Similarity_Matched_Submission_ID",
                table: "Assignment_Submissions");

            migrationBuilder.DropColumn(
                name: "Similarity_Score_Percent",
                table: "Assignment_Submissions");
        }
    }
}

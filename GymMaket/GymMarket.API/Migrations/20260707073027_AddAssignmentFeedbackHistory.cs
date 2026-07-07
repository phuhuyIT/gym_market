using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentFeedbackHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assignment_Feedback_Entries",
                columns: table => new
                {
                    Feedback_Entry_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Submission_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Author_User_ID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Author_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Author_Role = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Score_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment_Feedback_Entries", x => x.Feedback_Entry_ID);
                    table.ForeignKey(
                        name: "FK_Assignment_Feedback_Entries_Submission",
                        column: x => x.Submission_ID,
                        principalTable: "Assignment_Submissions",
                        principalColumn: "Submission_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Feedback_Entries_Submission_ID",
                table: "Assignment_Feedback_Entries",
                column: "Submission_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Feedback_Entries_Submission_ID_Created_At",
                table: "Assignment_Feedback_Entries",
                columns: new[] { "Submission_ID", "Created_At" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignment_Feedback_Entries");
        }
    }
}

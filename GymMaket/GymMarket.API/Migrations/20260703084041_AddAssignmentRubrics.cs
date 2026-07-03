using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentRubrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assignment_Rubric_Criteria",
                columns: table => new
                {
                    Criterion_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Assignment_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Points_Possible = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Criterion_Order = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment_Rubric_Criteria", x => x.Criterion_ID);
                    table.ForeignKey(
                        name: "FK_Assignment_Rubric_Criteria_Assignment",
                        column: x => x.Assignment_ID,
                        principalTable: "Course_Assignments",
                        principalColumn: "Assignment_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignment_Rubric_Scores",
                columns: table => new
                {
                    Rubric_Score_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Submission_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Criterion_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment_Rubric_Scores", x => x.Rubric_Score_ID);
                    table.ForeignKey(
                        name: "FK_Assignment_Rubric_Scores_Criterion",
                        column: x => x.Criterion_ID,
                        principalTable: "Assignment_Rubric_Criteria",
                        principalColumn: "Criterion_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assignment_Rubric_Scores_Submission",
                        column: x => x.Submission_ID,
                        principalTable: "Assignment_Submissions",
                        principalColumn: "Submission_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Rubric_Criteria_Assignment_ID",
                table: "Assignment_Rubric_Criteria",
                column: "Assignment_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Rubric_Criteria_Assignment_ID_Criterion_Order",
                table: "Assignment_Rubric_Criteria",
                columns: new[] { "Assignment_ID", "Criterion_Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Rubric_Scores_Criterion_ID",
                table: "Assignment_Rubric_Scores",
                column: "Criterion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Rubric_Scores_Submission_ID",
                table: "Assignment_Rubric_Scores",
                column: "Submission_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Rubric_Scores_Submission_ID_Criterion_ID",
                table: "Assignment_Rubric_Scores",
                columns: new[] { "Submission_ID", "Criterion_ID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignment_Rubric_Scores");

            migrationBuilder.DropTable(
                name: "Assignment_Rubric_Criteria");
        }
    }
}

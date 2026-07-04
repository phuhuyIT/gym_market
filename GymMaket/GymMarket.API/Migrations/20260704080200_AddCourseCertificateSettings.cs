using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseCertificateSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course_Certificate_Settings",
                columns: table => new
                {
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Is_Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Template_Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Certificate_Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Body_Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Accent_Color = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: false),
                    Required_Lecture_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 100m),
                    Require_Published_Quizzes = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Require_Published_Assignments = table.Column<bool>(type: "bit", nullable: false),
                    Required_Assignment_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Minimum_Final_Grade_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Certificate_Settings", x => x.Course_ID);
                    table.ForeignKey(
                        name: "FK_Course_Certificate_Settings_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course_Certificate_Settings");
        }
    }
}

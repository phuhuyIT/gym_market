using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseLiveSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course_Live_Sessions",
                columns: table => new
                {
                    Live_Session_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Starts_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Ends_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Meeting_Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Recording_Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Draft"),
                    Attendance_Required = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Published_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Live_Sessions", x => x.Live_Session_ID);
                    table.ForeignKey(
                        name: "FK_Course_Live_Sessions_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Live_Sessions_Course_ID",
                table: "Course_Live_Sessions",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Live_Sessions_Course_ID_Status_Starts_At",
                table: "Course_Live_Sessions",
                columns: new[] { "Course_ID", "Status", "Starts_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Live_Sessions_Starts_At",
                table: "Course_Live_Sessions",
                column: "Starts_At");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Live_Sessions_Status",
                table: "Course_Live_Sessions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course_Live_Sessions");
        }
    }
}

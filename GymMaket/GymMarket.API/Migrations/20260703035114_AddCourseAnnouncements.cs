using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseAnnouncements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course_Announcements",
                columns: table => new
                {
                    Announcement_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Created_By_User_ID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Created_By_Role = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Created_By_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Is_Pinned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Is_Published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Published_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Announcements", x => x.Announcement_ID);
                    table.ForeignKey(
                        name: "FK_Course_Announcements_AppUser",
                        column: x => x.Created_By_User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Course_Announcements_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Announcements_Course_ID",
                table: "Course_Announcements",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Announcements_Course_ID_Is_Pinned_Published_At",
                table: "Course_Announcements",
                columns: new[] { "Course_ID", "Is_Pinned", "Published_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Announcements_Course_ID_Is_Published_Published_At",
                table: "Course_Announcements",
                columns: new[] { "Course_ID", "Is_Published", "Published_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Announcements_Created_By_User_ID",
                table: "Course_Announcements",
                column: "Created_By_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Announcements_Is_Published",
                table: "Course_Announcements",
                column: "Is_Published");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Announcements_Published_At",
                table: "Course_Announcements",
                column: "Published_At");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course_Announcements");
        }
    }
}

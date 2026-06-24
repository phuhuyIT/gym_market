using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseOptionCourseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Course_ID",
                table: "Course_Options",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_Options_Course_ID",
                table: "Course_Options",
                column: "Course_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Options_Course",
                table: "Course_Options",
                column: "Course_ID",
                principalTable: "Courses",
                principalColumn: "Course_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Options_Course",
                table: "Course_Options");

            migrationBuilder.DropIndex(
                name: "IX_Course_Options_Course_ID",
                table: "Course_Options");

            migrationBuilder.DropColumn(
                name: "Course_ID",
                table: "Course_Options");
        }
    }
}

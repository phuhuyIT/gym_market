using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseOptionOptionId",
                table: "Course_Ratings",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_Ratings_CourseOptionOptionId",
                table: "Course_Ratings",
                column: "CourseOptionOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Ratings_Course_Options_CourseOptionOptionId",
                table: "Course_Ratings",
                column: "CourseOptionOptionId",
                principalTable: "Course_Options",
                principalColumn: "Option_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Ratings_Course_Options_CourseOptionOptionId",
                table: "Course_Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Course_Ratings_CourseOptionOptionId",
                table: "Course_Ratings");

            migrationBuilder.DropColumn(
                name: "CourseOptionOptionId",
                table: "Course_Ratings");
        }
    }
}

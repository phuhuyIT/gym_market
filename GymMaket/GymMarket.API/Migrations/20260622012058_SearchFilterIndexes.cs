using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SearchFilterIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Category_Experience",
                table: "Trainers",
                columns: new[] { "Category", "Experience" });

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Category_Rating",
                table: "Trainers",
                columns: new[] { "Category", "Rating" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_Health_Status",
                table: "Students",
                column: "Health_Status");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Health_Status_Created_At",
                table: "Students",
                columns: new[] { "Health_Status", "Created_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Course_ID_Payment_Status_Created_At",
                table: "Payments",
                columns: new[] { "Course_ID", "Payment_Status", "Created_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Payment_Type_Created_At",
                table: "Payments",
                columns: new[] { "Payment_Type", "Created_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Student_ID_Payment_Status_Created_At",
                table: "Payments",
                columns: new[] { "Student_ID", "Payment_Status", "Created_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Registration_Course_ID_Payment_Status",
                table: "Course_Registration",
                columns: new[] { "Course_ID", "Payment_Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Registration_Payment_Status",
                table: "Course_Registration",
                column: "Payment_Status");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Registration_Student_ID_Payment_Status",
                table: "Course_Registration",
                columns: new[] { "Student_ID", "Payment_Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trainers_Category_Experience",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_Category_Rating",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Students_Health_Status",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_Health_Status_Created_At",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Course_ID_Payment_Status_Created_At",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Payment_Type_Created_At",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Student_ID_Payment_Status_Created_At",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Course_Registration_Course_ID_Payment_Status",
                table: "Course_Registration");

            migrationBuilder.DropIndex(
                name: "IX_Course_Registration_Payment_Status",
                table: "Course_Registration");

            migrationBuilder.DropIndex(
                name: "IX_Course_Registration_Student_ID_Payment_Status",
                table: "Course_Registration");
        }
    }
}

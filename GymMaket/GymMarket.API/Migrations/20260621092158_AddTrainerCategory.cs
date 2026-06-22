using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Trainers",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [Trainers]
                SET [Category] = CASE
                    WHEN LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%crossfit%' THEN 'Crossfit'
                    WHEN LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%yoga%' THEN 'Yoga'
                    WHEN LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%cardio%'
                        OR LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%running%'
                        OR LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%cycling%'
                        OR LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%endurance%' THEN 'Cardio'
                    WHEN LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%strength%'
                        OR LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%powerlifting%'
                        OR LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%weight%'
                        OR LOWER(CONCAT(COALESCE([Certification], ''), ' ', COALESCE([Bio], ''), ' ', COALESCE([Desciption], ''))) LIKE '%hypertrophy%' THEN 'Strength'
                    ELSE NULL
                END
                WHERE [Category] IS NULL;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Category",
                table: "Trainers",
                column: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trainers_Category",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Trainers");
        }
    }
}

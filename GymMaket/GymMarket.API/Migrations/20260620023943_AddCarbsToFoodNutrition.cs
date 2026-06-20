using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCarbsToFoodNutrition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Carbs",
                table: "FoodNutritionUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Carbs",
                table: "FoodNutritions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.Sql("UPDATE FoodNutritionUsers SET Carbs = Sugars");
            migrationBuilder.Sql("UPDATE FoodNutritions SET Carbs = Sugars");

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Carbs",
                value: 13.800000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Carbs",
                value: 22.800000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 3,
                column: "Carbs",
                value: 11.800000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 4,
                column: "Carbs",
                value: 7.7000000000000002);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 5,
                column: "Carbs",
                value: 8.5);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 6,
                column: "Carbs",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 7,
                column: "Carbs",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 8,
                column: "Carbs",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 9,
                column: "Carbs",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 10,
                column: "Carbs",
                value: 0.20000000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 11,
                column: "Carbs",
                value: 1.1000000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 12,
                column: "Carbs",
                value: 3.6000000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 13,
                column: "Carbs",
                value: 4.7999999999999998);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 14,
                column: "Carbs",
                value: 1.3);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 15,
                column: "Carbs",
                value: 28.199999999999999);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 16,
                column: "Carbs",
                value: 23.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 17,
                column: "Carbs",
                value: 12.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 18,
                column: "Carbs",
                value: 41.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 19,
                column: "Carbs",
                value: 25.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 20,
                column: "Carbs",
                value: 20.699999999999999);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 21,
                column: "Carbs",
                value: 20.100000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 22,
                column: "Carbs",
                value: 21.300000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 23,
                column: "Carbs",
                value: 6.5999999999999996);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 24,
                column: "Carbs",
                value: 3.6000000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 25,
                column: "Carbs",
                value: 9.5999999999999996);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 26,
                column: "Carbs",
                value: 21.600000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 27,
                column: "Carbs",
                value: 20.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 28,
                column: "Carbs",
                value: 20.100000000000001);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 29,
                column: "Carbs",
                value: 23.699999999999999);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 30,
                column: "Carbs",
                value: 1.8999999999999999);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 31,
                column: "Carbs",
                value: 7.0);

            migrationBuilder.UpdateData(
                table: "FoodNutritions",
                keyColumn: "Id",
                keyValue: 32,
                column: "Carbs",
                value: 61.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carbs",
                table: "FoodNutritionUsers");

            migrationBuilder.DropColumn(
                name: "Carbs",
                table: "FoodNutritions");
        }
    }
}

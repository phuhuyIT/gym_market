using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class FoodNutritionFkIndexAndBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FoodNutritionUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "FoodNutritionId",
                table: "FoodNutritionUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NutritionBudgets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CalorieBudget = table.Column<double>(type: "float", nullable: false),
                    CarbsBudget = table.Column<double>(type: "float", nullable: false),
                    FatBudget = table.Column<double>(type: "float", nullable: false),
                    ProteinBudget = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionBudgets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutritionUsers_FoodNutritionId",
                table: "FoodNutritionUsers",
                column: "FoodNutritionId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutritionUsers_UserId_Date",
                table: "FoodNutritionUsers",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_NutritionBudgets_UserId",
                table: "NutritionBudgets",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodNutritionUsers_FoodNutritions_FoodNutritionId",
                table: "FoodNutritionUsers",
                column: "FoodNutritionId",
                principalTable: "FoodNutritions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodNutritionUsers_FoodNutritions_FoodNutritionId",
                table: "FoodNutritionUsers");

            migrationBuilder.DropTable(
                name: "NutritionBudgets");

            migrationBuilder.DropIndex(
                name: "IX_FoodNutritionUsers_FoodNutritionId",
                table: "FoodNutritionUsers");

            migrationBuilder.DropIndex(
                name: "IX_FoodNutritionUsers_UserId_Date",
                table: "FoodNutritionUsers");

            migrationBuilder.DropColumn(
                name: "FoodNutritionId",
                table: "FoodNutritionUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FoodNutritionUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}

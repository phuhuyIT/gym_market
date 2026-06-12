using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class FoodNutritionDateMealTypeAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "FoodNutritionUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealType",
                table: "FoodNutritionUsers",
                type: "nvarchar(max)",
                nullable: true);

            // FoodNutritions may already be populated (script.sql seeds ~870 foods
            // without explicit ids), so only apply the EF seed to empty databases.
            // Hand-written instead of the generated InsertData for that reason.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [FoodNutritions])
BEGIN
    SET IDENTITY_INSERT [FoodNutritions] ON;
    INSERT INTO [FoodNutritions] ([Id], [Name], [CaloricValue], [Fat], [Sugars], [Protein])
    VALUES
    (1,  N'Apple',                   52,  0.2,  10.4, 0.3),
    (2,  N'Banana',                  89,  0.3,  12.2, 1.1),
    (3,  N'Orange',                  47,  0.1,  9.4,  0.9),
    (4,  N'Strawberries',            32,  0.3,  4.9,  0.7),
    (5,  N'Avocado',                 160, 14.7, 0.7,  2.0),
    (6,  N'Grilled Chicken Breast',  165, 3.6,  0,    31.0),
    (7,  N'Lean Beef Steak',         250, 15.0, 0,    26.0),
    (8,  N'Baked Salmon',            208, 13.0, 0,    20.0),
    (9,  N'Canned Tuna (in Water)',  116, 0.8,  0,    25.5),
    (10, N'Cooked Shrimp',           99,  0.3,  0,    24.0),
    (11, N'Boiled Egg',              155, 11.0, 1.1,  13.0),
    (12, N'Plain Greek Yogurt',      59,  0.4,  3.2,  10.0),
    (13, N'Whole Milk',              61,  3.3,  5.1,  3.2),
    (14, N'Cheddar Cheese',          403, 33.0, 0.5,  25.0),
    (15, N'White Rice (Cooked)',     130, 0.3,  0.1,  2.7),
    (16, N'Brown Rice (Cooked)',     111, 0.9,  0.4,  2.6),
    (17, N'Oatmeal (Cooked)',        71,  1.5,  0.3,  2.5),
    (18, N'Whole Wheat Bread',       247, 3.4,  6.0,  13.0),
    (19, N'Pasta (Cooked)',          131, 1.1,  0.6,  5.0),
    (20, N'Baked Sweet Potato',      90,  0.2,  6.5,  2.0),
    (21, N'Boiled Potato',           87,  0.1,  0.9,  1.9),
    (22, N'Quinoa (Cooked)',         120, 1.9,  0.9,  4.4),
    (23, N'Broccoli',                34,  0.4,  1.7,  2.8),
    (24, N'Spinach',                 23,  0.4,  0.4,  2.9),
    (25, N'Carrot',                  41,  0.2,  4.7,  0.9),
    (26, N'Almonds',                 579, 49.9, 4.4,  21.2),
    (27, N'Peanut Butter',           588, 50.0, 9.2,  25.0),
    (28, N'Lentils (Cooked)',        116, 0.4,  1.8,  9.0),
    (29, N'Black Beans (Cooked)',    132, 0.5,  0.3,  8.9),
    (30, N'Tofu',                    76,  4.8,  0.6,  8.0),
    (31, N'Whey Protein Powder',     412, 7.0,  6.0,  71.0),
    (32, N'Dark Chocolate (70%)',    546, 31.0, 24.0, 7.9);
    SET IDENTITY_INSERT [FoodNutritions] OFF;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Matching on Id AND Name so rows that came from script.sql (different
            // foods at these ids) are never deleted on rollback.
            migrationBuilder.Sql(@"
DELETE FROM [FoodNutritions]
WHERE [Id] <= 32 AND [Name] IN (
    N'Apple', N'Banana', N'Orange', N'Strawberries', N'Avocado',
    N'Grilled Chicken Breast', N'Lean Beef Steak', N'Baked Salmon',
    N'Canned Tuna (in Water)', N'Cooked Shrimp', N'Boiled Egg',
    N'Plain Greek Yogurt', N'Whole Milk', N'Cheddar Cheese',
    N'White Rice (Cooked)', N'Brown Rice (Cooked)', N'Oatmeal (Cooked)',
    N'Whole Wheat Bread', N'Pasta (Cooked)', N'Baked Sweet Potato',
    N'Boiled Potato', N'Quinoa (Cooked)', N'Broccoli', N'Spinach', N'Carrot',
    N'Almonds', N'Peanut Butter', N'Lentils (Cooked)', N'Black Beans (Cooked)',
    N'Tofu', N'Whey Protein Powder', N'Dark Chocolate (70%)');
");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "FoodNutritionUsers");

            migrationBuilder.DropColumn(
                name: "MealType",
                table: "FoodNutritionUsers");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGradebookPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Grade_Category_ID",
                table: "Course_Quizzes",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Grade_Categories",
                columns: table => new
                {
                    Category_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Weight_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Category_Order = table.Column<int>(type: "int", nullable: false),
                    Is_Default = table.Column<bool>(type: "bit", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grade_Categories", x => x.Category_ID);
                    table.ForeignKey(
                        name: "FK_Grade_Categories_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Quizzes_Grade_Category_ID",
                table: "Course_Quizzes",
                column: "Grade_Category_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Grade_Categories_Course_ID",
                table: "Grade_Categories",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Grade_Categories_Course_ID_Category_Order",
                table: "Grade_Categories",
                columns: new[] { "Course_ID", "Category_Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Grade_Categories_Course_ID_Name",
                table: "Grade_Categories",
                columns: new[] { "Course_ID", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Quizzes_Grade_Category",
                table: "Course_Quizzes",
                column: "Grade_Category_ID",
                principalTable: "Grade_Categories",
                principalColumn: "Category_ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Quizzes_Grade_Category",
                table: "Course_Quizzes");

            migrationBuilder.DropTable(
                name: "Grade_Categories");

            migrationBuilder.DropIndex(
                name: "IX_Course_Quizzes_Grade_Category_ID",
                table: "Course_Quizzes");

            migrationBuilder.DropColumn(
                name: "Grade_Category_ID",
                table: "Course_Quizzes");
        }
    }
}

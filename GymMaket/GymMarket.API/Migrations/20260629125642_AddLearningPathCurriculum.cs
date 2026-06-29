using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningPathCurriculum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lectures_Course_ID",
                table: "Lectures");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "Lectures",
                newName: "Lecture_Order");

            migrationBuilder.AddColumn<string>(
                name: "Activity_Type",
                table: "Lectures",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                defaultValue: "Lesson");

            migrationBuilder.AddColumn<DateTime>(
                name: "Available_From",
                table: "Lectures",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Available_Until",
                table: "Lectures",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Lectures",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Preview",
                table: "Lectures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Published",
                table: "Lectures",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Module_ID",
                table: "Lectures",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prerequisite_Lecture_ID",
                table: "Lectures",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Unlock_After_Days",
                table: "Lectures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Course_Modules",
                columns: table => new
                {
                    Module_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Module_Order = table.Column<int>(type: "int", nullable: true),
                    Prerequisite_Module_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Unlock_After_Days = table.Column<int>(type: "int", nullable: true),
                    Available_From = table.Column<DateTime>(type: "datetime", nullable: true),
                    Available_Until = table.Column<DateTime>(type: "datetime", nullable: true),
                    Is_Published = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Modules", x => x.Module_ID);
                    table.ForeignKey(
                        name: "FK_Course_Modules_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Course_Modules_Prerequisite_Module",
                        column: x => x.Prerequisite_Module_ID,
                        principalTable: "Course_Modules",
                        principalColumn: "Module_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_Course_ID_Module_ID_Lecture_Order",
                table: "Lectures",
                columns: new[] { "Course_ID", "Module_ID", "Lecture_Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_Module_ID",
                table: "Lectures",
                column: "Module_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_Prerequisite_Lecture_ID",
                table: "Lectures",
                column: "Prerequisite_Lecture_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Modules_Course_ID",
                table: "Course_Modules",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Modules_Course_ID_Module_Order",
                table: "Course_Modules",
                columns: new[] { "Course_ID", "Module_Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Modules_Prerequisite_Module_ID",
                table: "Course_Modules",
                column: "Prerequisite_Module_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Course_Module",
                table: "Lectures",
                column: "Module_ID",
                principalTable: "Course_Modules",
                principalColumn: "Module_ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Prerequisite_Lecture",
                table: "Lectures",
                column: "Prerequisite_Lecture_ID",
                principalTable: "Lectures",
                principalColumn: "Lecture_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Course_Module",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Prerequisite_Lecture",
                table: "Lectures");

            migrationBuilder.DropTable(
                name: "Course_Modules");

            migrationBuilder.DropIndex(
                name: "IX_Lectures_Course_ID_Module_ID_Lecture_Order",
                table: "Lectures");

            migrationBuilder.DropIndex(
                name: "IX_Lectures_Module_ID",
                table: "Lectures");

            migrationBuilder.DropIndex(
                name: "IX_Lectures_Prerequisite_Lecture_ID",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Activity_Type",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Available_From",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Available_Until",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Is_Preview",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Is_Published",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Module_ID",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Prerequisite_Lecture_ID",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "Unlock_After_Days",
                table: "Lectures");

            migrationBuilder.RenameColumn(
                name: "Lecture_Order",
                table: "Lectures",
                newName: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_Course_ID",
                table: "Lectures",
                column: "Course_ID");
        }
    }
}

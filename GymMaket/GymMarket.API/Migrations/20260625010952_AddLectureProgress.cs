using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLectureProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lecture_Progress",
                columns: table => new
                {
                    Lecture_Progress_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Lecture_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Is_Completed = table.Column<bool>(type: "bit", nullable: false),
                    Completed_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecture_Progress", x => x.Lecture_Progress_ID);
                    table.ForeignKey(
                        name: "FK_Lecture_Progress_Lecture",
                        column: x => x.Lecture_ID,
                        principalTable: "Lectures",
                        principalColumn: "Lecture_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lecture_Progress_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_Progress_Lecture_ID",
                table: "Lecture_Progress",
                column: "Lecture_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_Progress_Student_ID_Lecture_ID",
                table: "Lecture_Progress",
                columns: new[] { "Student_ID", "Lecture_ID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lecture_Progress");
        }
    }
}

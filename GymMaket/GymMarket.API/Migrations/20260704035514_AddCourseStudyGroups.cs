using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseStudyGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course_Study_Groups",
                columns: table => new
                {
                    Study_Group_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Conversation_ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Kind = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "StudyGroup"),
                    Is_Default_Cohort = table.Column<bool>(type: "bit", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Created_By_User_ID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Study_Groups", x => x.Study_Group_ID);
                    table.ForeignKey(
                        name: "FK_Course_Study_Groups_Conversation",
                        column: x => x.Conversation_ID,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Course_Study_Groups_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Study_Groups_Conversation_ID",
                table: "Course_Study_Groups",
                column: "Conversation_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_Study_Groups_Course_ID",
                table: "Course_Study_Groups",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Study_Groups_Course_ID_Is_Active",
                table: "Course_Study_Groups",
                columns: new[] { "Course_ID", "Is_Active" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Study_Groups_Course_ID_Is_Default_Cohort",
                table: "Course_Study_Groups",
                columns: new[] { "Course_ID", "Is_Default_Cohort" },
                unique: true,
                filter: "[Is_Default_Cohort] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course_Study_Groups");
        }
    }
}

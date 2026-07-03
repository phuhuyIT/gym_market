using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseDiscussions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course_Discussion_Questions",
                columns: table => new
                {
                    Question_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Course_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Open"),
                    Is_Pinned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Accepted_Answer_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Last_Activity_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Discussion_Questions", x => x.Question_ID);
                    table.ForeignKey(
                        name: "FK_Course_Discussion_Questions_Course",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Course_Discussion_Questions_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Course_Discussion_Answers",
                columns: table => new
                {
                    Answer_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Question_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Author_User_ID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Author_Entity_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Author_Role = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Author_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Author_Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Is_Accepted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Discussion_Answers", x => x.Answer_ID);
                    table.ForeignKey(
                        name: "FK_Course_Discussion_Answers_Question",
                        column: x => x.Question_ID,
                        principalTable: "Course_Discussion_Questions",
                        principalColumn: "Question_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Answers_Author_Entity_ID",
                table: "Course_Discussion_Answers",
                column: "Author_Entity_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Answers_Author_User_ID",
                table: "Course_Discussion_Answers",
                column: "Author_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Answers_Is_Accepted",
                table: "Course_Discussion_Answers",
                column: "Is_Accepted");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Answers_Question_ID",
                table: "Course_Discussion_Answers",
                column: "Question_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Answers_Question_ID_Created_At",
                table: "Course_Discussion_Answers",
                columns: new[] { "Question_ID", "Created_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Questions_Course_ID",
                table: "Course_Discussion_Questions",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Questions_Course_ID_Is_Pinned_Last_Activity_At",
                table: "Course_Discussion_Questions",
                columns: new[] { "Course_ID", "Is_Pinned", "Last_Activity_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Questions_Course_ID_Status_Last_Activity_At",
                table: "Course_Discussion_Questions",
                columns: new[] { "Course_ID", "Status", "Last_Activity_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Questions_Last_Activity_At",
                table: "Course_Discussion_Questions",
                column: "Last_Activity_At");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Questions_Status",
                table: "Course_Discussion_Questions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Discussion_Questions_Student_ID",
                table: "Course_Discussion_Questions",
                column: "Student_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course_Discussion_Answers");

            migrationBuilder.DropTable(
                name: "Course_Discussion_Questions");
        }
    }
}

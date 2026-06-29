using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Student_Progress_Goals",
                columns: table => new
                {
                    Progress_Goal_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Target_Weight_Kg = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Target_Body_Fat_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Goal_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Active"),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student_Progress_Goals", x => x.Progress_Goal_ID);
                    table.ForeignKey(
                        name: "FK_Student_Progress_Goals_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Student_Progress_Logs",
                columns: table => new
                {
                    Progress_Log_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Logged_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Weight_Kg = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Body_Fat_Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Waist_Cm = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Chest_Cm = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Arm_Cm = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Hip_Cm = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Strength_Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student_Progress_Logs", x => x.Progress_Log_ID);
                    table.ForeignKey(
                        name: "FK_Student_Progress_Logs_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_Progress_Goals_Status",
                table: "Student_Progress_Goals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Progress_Goals_Student_ID",
                table: "Student_Progress_Goals",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Progress_Goals_Student_ID_Status",
                table: "Student_Progress_Goals",
                columns: new[] { "Student_ID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Student_Progress_Logs_Logged_At",
                table: "Student_Progress_Logs",
                column: "Logged_At");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Progress_Logs_Student_ID",
                table: "Student_Progress_Logs",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Progress_Logs_Student_ID_Logged_At",
                table: "Student_Progress_Logs",
                columns: new[] { "Student_ID", "Logged_At" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student_Progress_Goals");

            migrationBuilder.DropTable(
                name: "Student_Progress_Logs");
        }
    }
}

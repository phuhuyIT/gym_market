using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Membership_Plans",
                columns: table => new
                {
                    Plan_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", unicode: false, maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Duration_Days = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membership_Plans", x => x.Plan_ID);
                });

            migrationBuilder.CreateTable(
                name: "Student_Memberships",
                columns: table => new
                {
                    Membership_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Plan_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Student_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Starts_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Ends_At = table.Column<DateTime>(type: "datetime", nullable: false),
                    Cancelled_At = table.Column<DateTime>(type: "datetime", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Updated_At = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student_Memberships", x => x.Membership_ID);
                    table.ForeignKey(
                        name: "FK_Student_Memberships_Membership_Plan",
                        column: x => x.Plan_ID,
                        principalTable: "Membership_Plans",
                        principalColumn: "Plan_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Student_Memberships_Student",
                        column: x => x.Student_ID,
                        principalTable: "Students",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Membership_Plans_Is_Active",
                table: "Membership_Plans",
                column: "Is_Active");

            migrationBuilder.CreateIndex(
                name: "IX_Membership_Plans_Is_Active_Price",
                table: "Membership_Plans",
                columns: new[] { "Is_Active", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_Membership_Plans_Name",
                table: "Membership_Plans",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Memberships_Ends_At",
                table: "Student_Memberships",
                column: "Ends_At");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Memberships_Plan_ID",
                table: "Student_Memberships",
                column: "Plan_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Memberships_Status",
                table: "Student_Memberships",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Memberships_Student_ID",
                table: "Student_Memberships",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Memberships_Student_ID_Status_Ends_At",
                table: "Student_Memberships",
                columns: new[] { "Student_ID", "Status", "Ends_At" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student_Memberships");

            migrationBuilder.DropTable(
                name: "Membership_Plans");
        }
    }
}

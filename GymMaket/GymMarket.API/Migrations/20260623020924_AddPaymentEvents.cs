using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payment_Events",
                columns: table => new
                {
                    Payment_Event_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Payment_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Event_Type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Old_Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    New_Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Source = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Raw_Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_Events", x => x.Payment_Event_ID);
                    table.ForeignKey(
                        name: "FK_Payment_Events_Payment",
                        column: x => x.Payment_ID,
                        principalTable: "Payments",
                        principalColumn: "Payment_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Events_Created_At",
                table: "Payment_Events",
                column: "Created_At");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Events_Event_Type",
                table: "Payment_Events",
                column: "Event_Type");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Events_Payment_ID",
                table: "Payment_Events",
                column: "Payment_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Events_Payment_ID_Created_At",
                table: "Payment_Events",
                columns: new[] { "Payment_ID", "Created_At" });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Events_Source",
                table: "Payment_Events",
                column: "Source");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment_Events");
        }
    }
}

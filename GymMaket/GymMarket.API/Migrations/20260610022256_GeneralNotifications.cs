using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class GeneralNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The legacy table shape (string PK, Student-only FK) was never used by the
            // app; any stray rows are incompatible with the new schema (non-null UserId
            // FK to AspNetUsers), so clear them before the column surgery below.
            migrationBuilder.Sql("DELETE FROM Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Student",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Notifica__8C1160B56820950B",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_Student_ID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Notification_ID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Notification_Type",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Student_ID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Notification_Content",
                table: "Notifications",
                newName: "Content");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Notifications",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Notifications",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notifications",
                newName: "Notification_Content");

            migrationBuilder.AddColumn<string>(
                name: "Notification_ID",
                table: "Notifications",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notification_Type",
                table: "Notifications",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Student_ID",
                table: "Notifications",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "Notifications",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Notifica__8C1160B56820950B",
                table: "Notifications",
                column: "Notification_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Student_ID",
                table: "Notifications",
                column: "Student_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Student",
                table: "Notifications",
                column: "Student_ID",
                principalTable: "Students",
                principalColumn: "Student_ID");
        }
    }
}

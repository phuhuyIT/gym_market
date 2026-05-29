using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupChatSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserMessages",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "UserMessages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "text");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "ConversationParticipants",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "ConversationParticipants",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Member");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "ConversationParticipants");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Migrations.Persistence
{
    /// <inheritdoc />
    public partial class AddGroupInviteCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InviteCode",
                table: "Conversations",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<long>(
                name: "InvitedById",
                table: "ConversationMembers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_InviteCode",
                table: "Conversations",
                column: "InviteCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMembers_InvitedById",
                table: "ConversationMembers",
                column: "InvitedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMembers_Users_InvitedById",
                table: "ConversationMembers",
                column: "InvitedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMembers_Users_InvitedById",
                table: "ConversationMembers");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_InviteCode",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_ConversationMembers_InvitedById",
                table: "ConversationMembers");

            migrationBuilder.DropColumn(
                name: "InviteCode",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "InvitedById",
                table: "ConversationMembers");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStickerPublicId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_stickers_StickerId",
                table: "messages");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "stickers",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_stickers_PublicId",
                table: "stickers",
                column: "PublicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_stickers_StickerId",
                table: "messages",
                column: "StickerId",
                principalTable: "stickers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_stickers_StickerId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_stickers_PublicId",
                table: "stickers");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "stickers");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_stickers_StickerId",
                table: "messages",
                column: "StickerId",
                principalTable: "stickers",
                principalColumn: "Id");
        }
    }
}

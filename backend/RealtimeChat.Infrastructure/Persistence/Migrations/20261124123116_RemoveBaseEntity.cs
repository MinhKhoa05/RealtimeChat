using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserStickerCollections",
                table: "UserStickerCollections");

            migrationBuilder.DropIndex(
                name: "IX_UserStickerCollections_UserId_CollectionId",
                table: "UserStickerCollections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationMember",
                table: "ConversationMember");

            migrationBuilder.DropIndex(
                name: "IX_ConversationMember_ConversationId_MemberId",
                table: "ConversationMember");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserStickerCollections");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserStickerCollections");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Stickers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StickerCollections");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ShareTripSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ConversationMember");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ConversationMember");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Calls");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ConversationMember",
                newName: "JoinedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserStickerCollections",
                table: "UserStickerCollections",
                columns: new[] { "UserId", "CollectionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationMember",
                table: "ConversationMember",
                columns: new[] { "ConversationId", "MemberId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserStickerCollections",
                table: "UserStickerCollections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationMember",
                table: "ConversationMember");

            migrationBuilder.RenameColumn(
                name: "JoinedAt",
                table: "ConversationMember",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "UserStickerCollections",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserStickerCollections",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Stickers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StickerCollections",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShareTripSessions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Relationships",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RefreshTokens",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Posts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Messages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Medias",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Conversations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ConversationMember",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ConversationMember",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Calls",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserStickerCollections",
                table: "UserStickerCollections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationMember",
                table: "ConversationMember",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserStickerCollections_UserId_CollectionId",
                table: "UserStickerCollections",
                columns: new[] { "UserId", "CollectionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMember_ConversationId_MemberId",
                table: "ConversationMember",
                columns: new[] { "ConversationId", "MemberId" },
                unique: true);
        }
    }
}

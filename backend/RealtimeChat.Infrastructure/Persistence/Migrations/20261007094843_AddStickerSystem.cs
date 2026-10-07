using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStickerSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StickerId",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sticker_collections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SharedKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Visibility = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sticker_collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sticker_collections_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "stickers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CollectionId = table.Column<long>(type: "bigint", nullable: false),
                    Label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MediaId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stickers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stickers_media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stickers_sticker_collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "sticker_collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_sticker_collections",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CollectionId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_sticker_collections", x => new { x.UserId, x.CollectionId });
                    table.ForeignKey(
                        name: "FK_user_sticker_collections_sticker_collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "sticker_collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_sticker_collections_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_messages_StickerId",
                table: "messages",
                column: "StickerId");

            migrationBuilder.CreateIndex(
                name: "IX_sticker_collections_OwnerId",
                table: "sticker_collections",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_sticker_collections_SharedKey",
                table: "sticker_collections",
                column: "SharedKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stickers_CollectionId",
                table: "stickers",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_stickers_MediaId",
                table: "stickers",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_user_sticker_collections_CollectionId",
                table: "user_sticker_collections",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_stickers_StickerId",
                table: "messages",
                column: "StickerId",
                principalTable: "stickers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_stickers_StickerId",
                table: "messages");

            migrationBuilder.DropTable(
                name: "stickers");

            migrationBuilder.DropTable(
                name: "user_sticker_collections");

            migrationBuilder.DropTable(
                name: "sticker_collections");

            migrationBuilder.DropIndex(
                name: "IX_messages_StickerId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "StickerId",
                table: "messages");
        }
    }
}

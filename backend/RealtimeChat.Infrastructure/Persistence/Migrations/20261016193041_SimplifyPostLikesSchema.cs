using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Migrations.Persistence
{
    /// <inheritdoc />
    public partial class SimplifyPostLikesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes");

            migrationBuilder.DropIndex(
                name: "IX_PostLikes_PostId_UserId",
                table: "PostLikes");

            migrationBuilder.DropIndex(
                name: "IX_PostLikes_UserId_CreatedAt",
                table: "PostLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PostLikes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PostLikes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PostLikes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes",
                columns: new[] { "PostId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_PostLikes_UserId",
                table: "PostLikes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes");

            migrationBuilder.DropIndex(
                name: "IX_PostLikes_UserId",
                table: "PostLikes");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "PostLikes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PostLikes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PostLikes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostLikes_PostId_UserId",
                table: "PostLikes",
                columns: new[] { "PostId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostLikes_UserId_CreatedAt",
                table: "PostLikes",
                columns: new[] { "UserId", "CreatedAt" });
        }
    }
}

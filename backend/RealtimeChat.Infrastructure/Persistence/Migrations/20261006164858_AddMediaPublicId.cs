using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaPublicId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "media",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_media_PublicId",
                table: "media",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_media_PublicId",
                table: "media");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "media");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Migrations.Persistence
{
    /// <inheritdoc />
    public partial class SimplifyMentionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions");

            migrationBuilder.DropIndex(
                name: "IX_Mentions_MessageId",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Mentions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions",
                columns: new[] { "MessageId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Mentions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Mentions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Mentions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Start",
                table: "Mentions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Mentions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Mentions_MessageId",
                table: "Mentions",
                column: "MessageId");
        }
    }
}

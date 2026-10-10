using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddShareTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mention_messages_MessageId",
                table: "Mention");

            migrationBuilder.DropForeignKey(
                name: "FK_Mention_users_UserId",
                table: "Mention");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mention",
                table: "Mention");

            migrationBuilder.RenameTable(
                name: "Mention",
                newName: "Mentions");

            migrationBuilder.RenameIndex(
                name: "IX_Mention_UserId",
                table: "Mentions",
                newName: "IX_Mentions_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions",
                columns: new[] { "MessageId", "UserId" });

            migrationBuilder.CreateTable(
                name: "share_trip_sessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ConversationId = table.Column<long>(type: "bigint", nullable: false),
                    TripTitle = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinationLatitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    DestinationLongitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    EtaMinutes = table.Column<int>(type: "int", nullable: true),
                    EtaCalculatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EtaBaseLatitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    EtaBaseLongitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_share_trip_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_share_trip_sessions_conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_share_trip_sessions_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_live_locations",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    AccuracyMeters = table.Column<float>(type: "float", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_live_locations", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_user_live_locations_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_share_trip_sessions_ConversationId",
                table: "share_trip_sessions",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_share_trip_sessions_OwnerId",
                table: "share_trip_sessions",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_messages_MessageId",
                table: "Mentions",
                column: "MessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_users_UserId",
                table: "Mentions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_messages_MessageId",
                table: "Mentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_users_UserId",
                table: "Mentions");

            migrationBuilder.DropTable(
                name: "share_trip_sessions");

            migrationBuilder.DropTable(
                name: "user_live_locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions");

            migrationBuilder.RenameTable(
                name: "Mentions",
                newName: "Mention");

            migrationBuilder.RenameIndex(
                name: "IX_Mentions_UserId",
                table: "Mention",
                newName: "IX_Mention_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mention",
                table: "Mention",
                columns: new[] { "MessageId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Mention_messages_MessageId",
                table: "Mention",
                column: "MessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mention_users_UserId",
                table: "Mention",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeoCoordinate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationLatitude",
                table: "ShareTripSessions");

            migrationBuilder.DropColumn(
                name: "DestinationLongitude",
                table: "ShareTripSessions");

            migrationBuilder.DropColumn(
                name: "EtaBaseLatitude",
                table: "ShareTripSessions");

            migrationBuilder.DropColumn(
                name: "EtaBaseLongitude",
                table: "ShareTripSessions");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "UserLiveLocations",
                newName: "Coordinate_Longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "UserLiveLocations",
                newName: "Coordinate_Latitude");

            migrationBuilder.AlterColumn<decimal>(
                name: "Coordinate_Longitude",
                table: "UserLiveLocations",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldPrecision: 9,
                oldScale: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "Coordinate_Latitude",
                table: "UserLiveLocations",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldPrecision: 9,
                oldScale: 6);

            migrationBuilder.AddColumn<decimal>(
                name: "Destination_Latitude",
                table: "ShareTripSessions",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Destination_Longitude",
                table: "ShareTripSessions",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastEtaLocation_Latitude",
                table: "ShareTripSessions",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LastEtaLocation_Longitude",
                table: "ShareTripSessions",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination_Latitude",
                table: "ShareTripSessions");

            migrationBuilder.DropColumn(
                name: "Destination_Longitude",
                table: "ShareTripSessions");

            migrationBuilder.DropColumn(
                name: "LastEtaLocation_Latitude",
                table: "ShareTripSessions");

            migrationBuilder.DropColumn(
                name: "LastEtaLocation_Longitude",
                table: "ShareTripSessions");

            migrationBuilder.RenameColumn(
                name: "Coordinate_Longitude",
                table: "UserLiveLocations",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "Coordinate_Latitude",
                table: "UserLiveLocations",
                newName: "Latitude");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "UserLiveLocations",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "UserLiveLocations",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationLatitude",
                table: "ShareTripSessions",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationLongitude",
                table: "ShareTripSessions",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EtaBaseLatitude",
                table: "ShareTripSessions",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EtaBaseLongitude",
                table: "ShareTripSessions",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);
        }
    }
}

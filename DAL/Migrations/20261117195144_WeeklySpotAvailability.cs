using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class WeeklySpotAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpotAvailability_ParkingSpots_ParkingSpotSpotId",
                table: "SpotAvailability");

            migrationBuilder.DropIndex(
                name: "IX_SpotAvailability_ParkingSpotSpotId",
                table: "SpotAvailability");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "SpotAvailability");

            migrationBuilder.RenameColumn(
                name: "ParkingSpotSpotId",
                table: "SpotAvailability",
                newName: "DayOfWeek");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "SpotAvailability",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "SpotAvailability",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_SpotAvailability_SpotId",
                table: "SpotAvailability",
                column: "SpotId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpotAvailability_ParkingSpots_SpotId",
                table: "SpotAvailability",
                column: "SpotId",
                principalTable: "ParkingSpots",
                principalColumn: "SpotId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpotAvailability_ParkingSpots_SpotId",
                table: "SpotAvailability");

            migrationBuilder.DropIndex(
                name: "IX_SpotAvailability_SpotId",
                table: "SpotAvailability");

            migrationBuilder.RenameColumn(
                name: "DayOfWeek",
                table: "SpotAvailability",
                newName: "ParkingSpotSpotId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "SpotAvailability",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "SpotAvailability",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "SpotAvailability",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SpotAvailability_ParkingSpotSpotId",
                table: "SpotAvailability",
                column: "ParkingSpotSpotId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpotAvailability_ParkingSpots_ParkingSpotSpotId",
                table: "SpotAvailability",
                column: "ParkingSpotSpotId",
                principalTable: "ParkingSpots",
                principalColumn: "SpotId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

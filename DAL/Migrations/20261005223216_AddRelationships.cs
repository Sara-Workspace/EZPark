using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParkingSpotSpotId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParkingSpotSpotId",
                table: "DynamicPricingRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ParkingSpotSpotId",
                table: "Reservations",
                column: "ParkingSpotSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ReservationID",
                table: "Payment",
                column: "ReservationID");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicPricingRules_ParkingSpotSpotId",
                table: "DynamicPricingRules",
                column: "ParkingSpotSpotId");

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicPricingRules_ParkingSpots_ParkingSpotSpotId",
                table: "DynamicPricingRules",
                column: "ParkingSpotSpotId",
                principalTable: "ParkingSpots",
                principalColumn: "SpotId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Reservations_ReservationID",
                table: "Payment",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "ReservationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ParkingSpots_ParkingSpotSpotId",
                table: "Reservations",
                column: "ParkingSpotSpotId",
                principalTable: "ParkingSpots",
                principalColumn: "SpotId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DynamicPricingRules_ParkingSpots_ParkingSpotSpotId",
                table: "DynamicPricingRules");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Reservations_ReservationID",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ParkingSpots_ParkingSpotSpotId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ParkingSpotSpotId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Payment_ReservationID",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_DynamicPricingRules_ParkingSpotSpotId",
                table: "DynamicPricingRules");

            migrationBuilder.DropColumn(
                name: "ParkingSpotSpotId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ParkingSpotSpotId",
                table: "DynamicPricingRules");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payment_ReservationID",
                table: "Payment");

            migrationBuilder.RenameColumn(
                name: "ReservationID",
                table: "Reviews",
                newName: "ReservationId");

            migrationBuilder.RenameColumn(
                name: "DriverID",
                table: "Reviews",
                newName: "DriverId");

            migrationBuilder.RenameColumn(
                name: "ReviewsID",
                table: "Reviews",
                newName: "ReviewId");

            migrationBuilder.RenameColumn(
                name: "Londitude",
                table: "ParkingSpots",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "EvChrger",
                table: "ParkingSpots",
                newName: "EvCharger");

            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "ParkingSpots",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "PriceMulyiPlier",
                table: "DynamicPricingRules",
                newName: "PriceMultiplier");

            migrationBuilder.RenameColumn(
                name: "EvExtraCharg",
                table: "DynamicPricingRules",
                newName: "EvExtraCharge");

            migrationBuilder.AddColumn<int>(
                name: "ParkingSpotSpotId",
                table: "SpotAvailability",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    DriverID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethodID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.DriverID);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    OwnerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.OwnerID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpotAvailability_ParkingSpotSpotId",
                table: "SpotAvailability",
                column: "ParkingSpotSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DriverId",
                table: "Reviews",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OwnerId",
                table: "Reviews",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReservationId",
                table: "Reviews",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_DriverID",
                table: "Reservations",
                column: "DriverID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ReservationID",
                table: "Payment",
                column: "ReservationID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpots_OwnerId",
                table: "ParkingSpots",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpots_Owners_OwnerId",
                table: "ParkingSpots",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "OwnerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Drivers_DriverID",
                table: "Reservations",
                column: "DriverID",
                principalTable: "Drivers",
                principalColumn: "DriverID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Drivers_DriverId",
                table: "Reviews",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Owners_OwnerId",
                table: "Reviews",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "OwnerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Reservations_ReservationId",
                table: "Reviews",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "ReservationID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpotAvailability_ParkingSpots_ParkingSpotSpotId",
                table: "SpotAvailability",
                column: "ParkingSpotSpotId",
                principalTable: "ParkingSpots",
                principalColumn: "SpotId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpots_Owners_OwnerId",
                table: "ParkingSpots");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Drivers_DriverID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Drivers_DriverId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Owners_OwnerId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Reservations_ReservationId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_SpotAvailability_ParkingSpots_ParkingSpotSpotId",
                table: "SpotAvailability");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_SpotAvailability_ParkingSpotSpotId",
                table: "SpotAvailability");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_DriverId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_OwnerId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReservationId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_DriverID",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Payment_ReservationID",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpots_OwnerId",
                table: "ParkingSpots");

            migrationBuilder.DropColumn(
                name: "ParkingSpotSpotId",
                table: "SpotAvailability");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "Reviews",
                newName: "ReservationID");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "Reviews",
                newName: "DriverID");

            migrationBuilder.RenameColumn(
                name: "ReviewId",
                table: "Reviews",
                newName: "ReviewsID");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "ParkingSpots",
                newName: "Londitude");

            migrationBuilder.RenameColumn(
                name: "EvCharger",
                table: "ParkingSpots",
                newName: "EvChrger");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "ParkingSpots",
                newName: "Adress");

            migrationBuilder.RenameColumn(
                name: "PriceMultiplier",
                table: "DynamicPricingRules",
                newName: "PriceMulyiPlier");

            migrationBuilder.RenameColumn(
                name: "EvExtraCharge",
                table: "DynamicPricingRules",
                newName: "EvExtraCharg");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Payment",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ReservationID",
                table: "Payment",
                column: "ReservationID");
        }
    }
}

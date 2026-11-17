using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSpotChecksAndFines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PendingFinePoints",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SpotChecks",
                columns: table => new
                {
                    SpotCheckId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpotId = table.Column<int>(type: "int", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestedByEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FinedUserId = table.Column<int>(type: "int", nullable: true),
                    FinePoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotChecks", x => x.SpotCheckId);
                    table.ForeignKey(
                        name: "FK_SpotChecks_ParkingSpots_SpotId",
                        column: x => x.SpotId,
                        principalTable: "ParkingSpots",
                        principalColumn: "SpotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpotChecks_SpotId",
                table: "SpotChecks",
                column: "SpotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpotChecks");

            migrationBuilder.DropColumn(
                name: "PendingFinePoints",
                table: "Users");
        }
    }
}

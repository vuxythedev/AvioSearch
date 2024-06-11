using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightSearches",
                columns: table => new
                {
                    SearchKey = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSearches", x => x.SearchKey);
                });

            migrationBuilder.CreateTable(
                name: "FlightOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DestinationCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Itineraries = table.Column<int>(type: "int", nullable: true),
                    NumberOfBookableSeats = table.Column<int>(type: "int", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FlightSearchKey = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightOffers_FlightSearches_FlightSearchKey",
                        column: x => x.FlightSearchKey,
                        principalTable: "FlightSearches",
                        principalColumn: "SearchKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightOffers_FlightSearchKey",
                table: "FlightOffers",
                column: "FlightSearchKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightOffers");

            migrationBuilder.DropTable(
                name: "FlightSearches");
        }
    }
}

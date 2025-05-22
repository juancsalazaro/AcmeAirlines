using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AcmeAirlines.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlightNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    OriginCityId = table.Column<int>(type: "integer", nullable: false),
                    DestinationCityId = table.Column<int>(type: "integer", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalSeats = table.Column<int>(type: "integer", nullable: false),
                    AvailableSeats = table.Column<int>(type: "integer", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flights_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flights_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlightId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AvailableSeats = table.Column<int>(type: "integer", nullable: false),
                    IsRefundable = table.Column<bool>(type: "boolean", nullable: false),
                    IncludesCheckedBaggage = table.Column<bool>(type: "boolean", nullable: false),
                    ChangeFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fares_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReservationCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FlightId = table.Column<int>(type: "integer", nullable: false),
                    PassengerId = table.Column<int>(type: "integer", nullable: false),
                    FareId = table.Column<int>(type: "integer", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SeatNumber = table.Column<string>(type: "text", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Fares_FareId",
                        column: x => x.FareId,
                        principalTable: "Fares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Passengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passengers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Code", "Country", "Name" },
                values: new object[,]
                {
                    { 1, "BOG", "Colombia", "Bogotá" },
                    { 2, "MDE", "Colombia", "Medellín" },
                    { 3, "CLO", "Colombia", "Cali" },
                    { 4, "MIA", "Estados Unidos", "Miami" },
                    { 5, "MAD", "España", "Madrid" }
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Id", "ArrivalTime", "AvailableSeats", "BasePrice", "DepartureTime", "DestinationCityId", "FlightNumber", "OriginCityId", "Status", "TotalSeats" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 1, 9, 0, 0, 0, DateTimeKind.Utc), 180, 250000m, new DateTime(2025, 5, 1, 8, 0, 0, 0, DateTimeKind.Utc), 2, "AM101", 1, "Scheduled", 180 },
                    { 2, new DateTime(2025, 5, 1, 11, 0, 0, 0, DateTimeKind.Utc), 180, 270000m, new DateTime(2025, 5, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, "AM102", 2, "Scheduled", 180 },
                    { 3, new DateTime(2025, 5, 2, 10, 0, 0, 0, DateTimeKind.Utc), 220, 850000m, new DateTime(2025, 5, 2, 6, 0, 0, 0, DateTimeKind.Utc), 4, "AM201", 1, "Scheduled", 220 }
                });

            migrationBuilder.InsertData(
                table: "Fares",
                columns: new[] { "Id", "AvailableSeats", "ChangeFee", "Description", "FlightId", "IncludesCheckedBaggage", "IsRefundable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 120, 100000m, "Tarifa básica sin equipaje facturado", 1, false, false, "Económica", 250000m },
                    { 2, 50, 50000m, "Tarifa con equipaje y cambios permitidos", 1, true, true, "Ejecutiva", 350000m },
                    { 3, 10, 0m, "Tarifa completa con todos los servicios", 1, true, true, "Premium", 450000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fares_FlightId",
                table: "Fares",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DestinationCityId",
                table: "Flights",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_OriginCityId",
                table: "Flights",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FareId",
                table: "Reservations",
                column: "FareId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FlightId",
                table: "Reservations",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PassengerId",
                table: "Reservations",
                column: "PassengerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Fares");

            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}

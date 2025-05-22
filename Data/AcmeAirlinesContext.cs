using Microsoft.EntityFrameworkCore;
using AcmeAirlines.Models;

namespace AcmeAirlines.Data
{
    public class AcmeAirlinesContext : DbContext
    {
        public AcmeAirlinesContext(DbContextOptions<AcmeAirlinesContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Fare> Fares { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.OriginCity)
                .WithMany(c => c.OriginFlights)
                .HasForeignKey(f => f.OriginCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.DestinationCity)
                .WithMany(c => c.DestinationFlights)
                .HasForeignKey(f => f.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Bogotá", Code = "BOG", Country = "Colombia" },
                new City { Id = 2, Name = "Medellín", Code = "MDE", Country = "Colombia" },
                new City { Id = 3, Name = "Cali", Code = "CLO", Country = "Colombia" },
                new City { Id = 4, Name = "Miami", Code = "MIA", Country = "Estados Unidos" },
                new City { Id = 5, Name = "Madrid", Code = "MAD", Country = "España" }
            );

            modelBuilder.Entity<Flight>().HasData(
                new Flight
                {
                    Id = 1,
                    FlightNumber = "AM101",
                    OriginCityId = 1,
                    DestinationCityId = 2,
                    DepartureTime = new DateTime(2025, 5, 1, 8, 0, 0, DateTimeKind.Utc),
                    ArrivalTime = new DateTime(2025, 5, 1, 9, 0, 0, DateTimeKind.Utc),
                    TotalSeats = 180,
                    AvailableSeats = 180,
                    BasePrice = 250000m
                },
                new Flight
                {
                    Id = 2,
                    FlightNumber = "AM102",
                    OriginCityId = 2,
                    DestinationCityId = 1,
                    DepartureTime = new DateTime(2025, 5, 1, 10, 0, 0, DateTimeKind.Utc),
                    ArrivalTime = new DateTime(2025, 5, 1, 11, 0, 0, DateTimeKind.Utc),
                    TotalSeats = 180,
                    AvailableSeats = 180,
                    BasePrice = 270000m
                },
                new Flight
                {
                    Id = 3,
                    FlightNumber = "AM201",
                    OriginCityId = 1,
                    DestinationCityId = 4,
                    DepartureTime = new DateTime(2025, 5, 2, 6, 0, 0, DateTimeKind.Utc),
                    ArrivalTime = new DateTime(2025, 5, 2, 10, 0, 0, DateTimeKind.Utc),
                    TotalSeats = 220,
                    AvailableSeats = 220,
                    BasePrice = 850000m
                }
            );

            modelBuilder.Entity<Fare>().HasData(
                new Fare
                {
                    Id = 1,
                    FlightId = 1,
                    Name = "Económica",
                    Description = "Tarifa básica sin equipaje facturado",
                    Price = 250000m,
                    AvailableSeats = 120,
                    IsRefundable = false,
                    IncludesCheckedBaggage = false,
                    ChangeFee = 100000m
                },
                new Fare
                {
                    Id = 2,
                    FlightId = 1,
                    Name = "Ejecutiva",
                    Description = "Tarifa con equipaje y cambios permitidos",
                    Price = 350000m,
                    AvailableSeats = 50,
                    IsRefundable = true,
                    IncludesCheckedBaggage = true,
                    ChangeFee = 50000m
                },
                new Fare
                {
                    Id = 3,
                    FlightId = 1,
                    Name = "Premium",
                    Description = "Tarifa completa con todos los servicios",
                    Price = 450000m,
                    AvailableSeats = 10,
                    IsRefundable = true,
                    IncludesCheckedBaggage = true,
                    ChangeFee = 0m
                }
            );
        }
    }
}
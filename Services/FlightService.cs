using Microsoft.EntityFrameworkCore;
using AcmeAirlines.Data;
using AcmeAirlines.DTOs;
using AcmeAirlines.Models;

namespace AcmeAirlines.Services
{
    public class FlightService : IFlightService
    {
        private readonly AcmeAirlinesContext _context;

        public FlightService(AcmeAirlinesContext context)
        {
            _context = context;
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<List<FlightResultDto>> SearchFlightsAsync(FlightSearchDto searchDto)
        {
            // Validar que la fecha de salida no sea anterior a la fecha actual
            if (searchDto.DepartureDate.Date < DateTime.Today)
            {
                throw new ArgumentException("La fecha de salida no puede ser anterior a la fecha actual.");
            }

            // Validar que el origen y destino sean diferentes
            if (searchDto.OriginCityId == searchDto.DestinationCityId)
            {
                throw new ArgumentException("La ciudad de origen y destino no pueden ser la misma.");
            }

            // Buscar vuelos que coincidan con el criterio de búsqueda
            var query = _context.Flights
                .Include(f => f.OriginCity)
                .Include(f => f.DestinationCity)
                .Include(f => f.Fares)
                .Where(f => f.OriginCityId == searchDto.OriginCityId &&
                            f.DestinationCityId == searchDto.DestinationCityId &&
                            f.DepartureTime.Date == searchDto.DepartureDate.Date &&
                            f.AvailableSeats >= searchDto.Passengers &&
                            f.Status != "Cancelled");

            var flights = await query.ToListAsync();

            return flights.Select(flight => new FlightResultDto
            {
                FlightId = flight.Id,
                FlightNumber = flight.FlightNumber,
                OriginCity = flight.OriginCity.Name,
                OriginCityCode = flight.OriginCity.Code,
                DestinationCity = flight.DestinationCity.Name,
                DestinationCityCode = flight.DestinationCity.Code,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                MinPrice = flight.Fares.Count > 0 ? flight.Fares.Min(f => f.Price) : flight.BasePrice,
                AvailableSeats = flight.AvailableSeats,
                Fares = flight.Fares.Select(fare => new FareDto
                {
                    FareId = fare.Id,
                    Name = fare.Name,
                    Description = fare.Description,
                    Price = fare.Price,
                    IsRefundable = fare.IsRefundable,
                    IncludesCheckedBaggage = fare.IncludesCheckedBaggage,
                    ChangeFee = fare.ChangeFee
                }).ToList()
            }).ToList();
        }

        public async Task<FlightResultDto> GetFlightDetailsAsync(int flightId)
        {
            var flight = await _context.Flights
                .Include(f => f.OriginCity)
                .Include(f => f.DestinationCity)
                .Include(f => f.Fares)
                .FirstOrDefaultAsync(f => f.Id == flightId);

            if (flight == null)
            {
                return null;
            }

            // Aquí se crearía normalmente la lista de asientos desde una tabla en la base de datos
            // Por simplicidad, generaremos datos ficticios de asientos
            var seats = GenerateDummySeats(flight);

            return new FlightResultDto
            {
                FlightId = flight.Id,
                FlightNumber = flight.FlightNumber,
                OriginCity = flight.OriginCity.Name,
                OriginCityCode = flight.OriginCity.Code,
                DestinationCity = flight.DestinationCity.Name,
                DestinationCityCode = flight.DestinationCity.Code,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                MinPrice = flight.Fares.Count > 0 ? flight.Fares.Min(f => f.Price) : flight.BasePrice,
                AvailableSeats = flight.AvailableSeats,
                Fares = flight.Fares.Select(fare => new FareDto
                {
                    FareId = fare.Id,
                    Name = fare.Name,
                    Description = fare.Description,
                    Price = fare.Price,
                    IsRefundable = fare.IsRefundable,
                    IncludesCheckedBaggage = fare.IncludesCheckedBaggage,
                    ChangeFee = fare.ChangeFee
                }).ToList(),
                Seats = seats // Asignar los asientos generados
            };
        }

        // Método para generar datos ficticios de asientos
        private List<SeatDto> GenerateDummySeats(Flight flight)
        {
            var seats = new List<SeatDto>();
            int totalRows = flight.OriginCity.Country != flight.DestinationCity.Country ? 30 : 20;

            for (int row = 1; row <= totalRows; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    char seatLetter = (char)('A' + col);
                    string seatNumber = $"{row}{seatLetter}";

                    string seatClass = "Economy";
                    decimal? extraCharge = null;

                    if (row <= 2)
                    {
                        seatClass = "First";
                        extraCharge = 150000m;
                    }
                    else if (row <= 8)
                    {
                        seatClass = "Business";
                        extraCharge = 80000m;
                    }
                    else if (row == 14 || row == 15) // Filas de salida de emergencia
                    {
                        extraCharge = 40000m;
                    }

                    var seat = new SeatDto
                    {
                        SeatNumber = seatNumber,
                        Row = row,
                        Column = col,
                        IsAvailable = true,
                        IsEmergencyExit = (row == 14 || row == 15),
                        IsAisle = (col == 2 || col == 3), // Asientos C y D son de pasillo
                        IsWindow = (col == 0 || col == 5), // Asientos A y F son de ventana
                        SeatClass = seatClass,
                        ExtraCharge = extraCharge,
                        IsSelected = false
                    };

                    seats.Add(seat);
                }
            }

            // Marcar algunos asientos como no disponibles (aleatoriamente)
            Random random = new Random();
            int seatsToMakeUnavailable = seats.Count / 4; // Aproximadamente 25% de asientos no disponibles

            for (int i = 0; i < seatsToMakeUnavailable; i++)
            {
                int randomIndex = random.Next(seats.Count);
                seats[randomIndex].IsAvailable = false;
            }

            return seats;
        }
    }
}
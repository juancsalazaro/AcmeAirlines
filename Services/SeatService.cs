using Microsoft.EntityFrameworkCore;
using AcmeAirlines.Data;
using AcmeAirlines.DTOs;
using AcmeAirlines.Models;

namespace AcmeAirlines.Services
{
    public class SeatService : ISeatService
    {
        private readonly AcmeAirlinesContext _context;

        public SeatService(AcmeAirlinesContext context)
        {
            _context = context;
        }

        public async Task<SeatMapDto> GetSeatMapAsync(int flightId)
        {
            // Obtener información del vuelo
            var flight = await _context.Flights
                .Include(f => f.OriginCity)
                .Include(f => f.DestinationCity)
                .FirstOrDefaultAsync(f => f.Id == flightId);

            if (flight == null)
            {
                return null;
            }

            // En un sistema real, la configuración de asientos se obtendría de la base de datos
            // Para esta implementación, generamos un mapa de asientos ficticio
            var seatMap = GenerateDummySeatMap(flight);

            // En un sistema real, también marcaríamos los asientos ya reservados como no disponibles
            // Para esta implementación, generamos aleatoriamente algunos asientos no disponibles
            MarkRandomSeatsAsUnavailable(seatMap);

            return seatMap;
        }

        private SeatMapDto GenerateDummySeatMap(Flight flight)
        {
            // Configuración ficticia basada en el tipo de vuelo (nacional/internacional)
            bool isInternational = flight.OriginCity.Country != flight.DestinationCity.Country;

            int totalRows = isInternational ? 30 : 20;
            int seatsPerRow = 6; // A, B, C, D, E, F

            var seatMap = new SeatMapDto
            {
                FlightId = flight.Id,
                FlightNumber = flight.FlightNumber,
                TotalRows = totalRows,
                SeatsPerRow = seatsPerRow,
                AircraftType = isInternational ? "Boeing 737-800" : "Airbus A320",
                Seats = new List<SeatDto>()
            };

            // Generamos todos los asientos
            for (int row = 1; row <= totalRows; row++)
            {
                for (int col = 0; col < seatsPerRow; col++)
                {
                    char seatLetter = (char)('A' + col);

                    // Determinar clase del asiento
                    string seatClass = "Economy";
                    decimal? extraCharge = null;

                    if (row <= 2)
                    {
                        seatClass = "First";
                        extraCharge = 150000m; // Cargo extra para primera clase
                    }
                    else if (row <= 8)
                    {
                        seatClass = "Business";
                        extraCharge = 80000m; // Cargo extra para clase ejecutiva
                    }
                    else if (row == 14 || row == 15) // Filas de salida de emergencia
                    {
                        extraCharge = 40000m; // Cargo extra para asientos con más espacio
                    }

                    var seat = new SeatDto
                    {
                        SeatNumber = $"{row}{seatLetter}",
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

                    seatMap.Seats.Add(seat);
                }
            }

            return seatMap;
        }

        private void MarkRandomSeatsAsUnavailable(SeatMapDto seatMap)
        {
            // Para simular asientos ya reservados, marcamos algunos como no disponibles
            Random random = new Random();
            int seatsToMakeUnavailable = seatMap.Seats.Count / 4; // Aproximadamente 25% de asientos no disponibles

            for (int i = 0; i < seatsToMakeUnavailable; i++)
            {
                int randomIndex = random.Next(seatMap.Seats.Count);
                seatMap.Seats[randomIndex].IsAvailable = false;
            }
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, string seatNumber)
        {
            // En un sistema real, consultaríamos la disponibilidad en la base de datos
            // Para esta implementación simplificada, siempre retornamos true
            return true;
        }

        public async Task<List<string>> GetSelectedSeatsAsync(int flightId, int reservationId)
        {
            // En un sistema real, consultaríamos los asientos ya seleccionados en la reserva
            // Para esta implementación, retornamos una lista vacía
            return new List<string>();
        }

        public async Task<decimal> CalculateSeatExtraChargesAsync(List<string> seatNumbers, int flightId)
        {
            // Obtenemos el mapa de asientos para consultar los cargos extra
            var seatMap = await GetSeatMapAsync(flightId);

            if (seatMap == null)
            {
                return 0;
            }

            decimal totalCharge = 0;

            foreach (var seatNumber in seatNumbers)
            {
                var seat = seatMap.Seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
                if (seat != null && seat.ExtraCharge.HasValue)
                {
                    totalCharge += seat.ExtraCharge.Value;
                }
            }

            return totalCharge;
        }

        public async Task<bool> ValidateSeatsForFareTypeAsync(List<string> seatNumbers, int flightId, int fareId)
        {
            // Obtener información de la tarifa
            var fare = await _context.Fares.FindAsync(fareId);
            if (fare == null)
            {
                return false;
            }

            // Obtener el mapa de asientos
            var seatMap = await GetSeatMapAsync(flightId);
            if (seatMap == null)
            {
                return false;
            }

            // Validar según el tipo de tarifa
            foreach (var seatNumber in seatNumbers)
            {
                var seat = seatMap.Seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
                if (seat == null || !seat.IsAvailable)
                {
                    return false; // Asiento no existe o no disponible
                }

                // Validaciones según tipo de tarifa
                // En tarifa económica, no se permiten asientos de primera clase o business
                if (fare.Name == "Económica" && (seat.SeatClass == "First" || seat.SeatClass == "Business"))
                {
                    return false;
                }

                // En tarifa ejecutiva, no se permiten asientos de primera clase
                if (fare.Name == "Ejecutiva" && seat.SeatClass == "First")
                {
                    return false;
                }
            }

            return true;
        }
    }
}
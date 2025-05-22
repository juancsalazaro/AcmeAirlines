using Microsoft.EntityFrameworkCore;
using AcmeAirlines.Data;
using AcmeAirlines.DTOs;
using AcmeAirlines.Models;

namespace AcmeAirlines.Services
{
    public class BookingService : IBookingService
    {
        private readonly AcmeAirlinesContext _context;
        private readonly IFlightService _flightService;
        private readonly ISeatService _seatService;
        private readonly IAdditionalServicesService _additionalServicesService;

        public BookingService(
            AcmeAirlinesContext context,
            IFlightService flightService,
            ISeatService seatService,
            IAdditionalServicesService additionalServicesService)
        {
            _context = context;
            _flightService = flightService;
            _seatService = seatService;
            _additionalServicesService = additionalServicesService;
        }

        public async Task<BookingSummaryDto> GetBookingSummaryAsync(
            int flightId,
            int fareId,
            PassengerListDto passengers,
            SeatSelectionDto seatSelection,
            SelectedServicesDto selectedServices)
        {
            // Obtener información del vuelo
            var flight = await _flightService.GetFlightDetailsAsync(flightId);
            if (flight == null)
            {
                return null;
            }

            // Obtener información de la tarifa
            var selectedFare = flight.Fares.FirstOrDefault(f => f.FareId == fareId);
            if (selectedFare == null)
            {
                return null;
            }

            // Calcular cargos por asientos
            decimal seatCharges = await _seatService.CalculateSeatExtraChargesAsync(
                seatSelection?.SelectedSeats ?? new List<string>(),
                flightId);

            // Calcular cargos por servicios adicionales
            decimal additionalServiceCharges = await _additionalServicesService.CalculateServicesCostAsync(
                selectedServices ?? new SelectedServicesDto { FlightId = flightId });

            // Calcular impuestos y tasas
            decimal taxesAndFees = await CalculateTaxesAndFeesAsync(
                flightId,
                fareId,
                passengers?.TotalPassengers ?? 1);

            // Calcular cargos desglosados por tipo
            decimal baggageCharges = 0;
            decimal mealCharges = 0;
            decimal otherServiceCharges = 0;

            // Desglosar los cargos de servicios adicionales por tipo
            if (selectedServices != null)
            {
                var availableServices = await _additionalServicesService.GetAvailableServicesAsync(flightId, fareId);

                // Calcular cargos de equipaje
                foreach (var baggageId in selectedServices.SelectedBaggageIds)
                {
                    var baggage = availableServices.BaggageOptions.FirstOrDefault(b => b.Id == baggageId);
                    if (baggage != null)
                    {
                        baggageCharges += baggage.Price;
                    }
                }

                // Calcular cargos de comidas
                foreach (var mealId in selectedServices.SelectedMealIds)
                {
                    var meal = availableServices.MealOptions.FirstOrDefault(m => m.Id == mealId);
                    if (meal != null)
                    {
                        mealCharges += meal.Price;
                    }
                }

                // Calcular cargos de otros servicios
                foreach (var serviceId in selectedServices.SelectedExtraServiceIds)
                {
                    var service = availableServices.ExtraServices.FirstOrDefault(s => s.Id == serviceId);
                    if (service != null)
                    {
                        otherServiceCharges += service.Price;
                    }
                }
            }

            // Crear objeto de resumen de precios
            var pricingSummary = new PricingSummaryDto
            {
                BaseFarePrice = selectedFare.Price,
                PassengerCount = passengers?.TotalPassengers ?? 1,
                SeatUpgradeCharges = seatCharges,
                BaggageCharges = baggageCharges,
                MealCharges = mealCharges,
                OtherServiceCharges = otherServiceCharges,
                TaxesAndFees = taxesAndFees
            };

            // Crear resumen completo de la reserva
            var bookingSummary = new BookingSummaryDto
            {
                FlightInfo = flight,
                SelectedFare = selectedFare,
                Passengers = passengers,
                SeatSelection = seatSelection,
                AdditionalServices = await _additionalServicesService.GetAvailableServicesAsync(flightId, fareId),
                PricingSummary = pricingSummary
            };

            return bookingSummary;
        }

        public async Task<string> GenerateReservationCodeAsync()
        {
            // Generar un código de reserva alfanumérico de 6 caracteres
            // En un sistema real, verificaríamos que no exista ya en la base de datos
            Random random = new Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Excluimos caracteres confusos como I, O, 0, 1

            string code = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return code;
        }

        public async Task<decimal> CalculateTaxesAndFeesAsync(int flightId, int fareId, int passengerCount)
        {
            // Obtener información del vuelo para calcular tasas e impuestos
            var flight = await _context.Flights
                .Include(f => f.OriginCity)
                .Include(f => f.DestinationCity)
                .FirstOrDefaultAsync(f => f.Id == flightId);

            if (flight == null)
            {
                return 0;
            }

            bool isInternational = flight.OriginCity.Country != flight.DestinationCity.Country;

            // Tarifas base por pasajero
            decimal baseTax = isInternational ? 120000m : 40000m;

            // En un sistema real, las tasas pueden variar según aeropuertos, países, etc.
            return baseTax * passengerCount;
        }
    }
}
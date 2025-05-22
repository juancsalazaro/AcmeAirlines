using AcmeAirlines.DTOs;

namespace AcmeAirlines.Services
{
    public interface IBookingService
    {
        Task<BookingSummaryDto> GetBookingSummaryAsync(
            int flightId,
            int fareId,
            PassengerListDto passengers,
            SeatSelectionDto seatSelection,
            SelectedServicesDto selectedServices);

        Task<string> GenerateReservationCodeAsync();
        Task<decimal> CalculateTaxesAndFeesAsync(int flightId, int fareId, int passengerCount);
    }
}
using AcmeAirlines.DTOs;

namespace AcmeAirlines.Services
{
    public interface ISeatService
    {
        Task<SeatMapDto> GetSeatMapAsync(int flightId);
        Task<bool> IsSeatAvailableAsync(int flightId, string seatNumber);
        Task<List<string>> GetSelectedSeatsAsync(int flightId, int reservationId);
        Task<decimal> CalculateSeatExtraChargesAsync(List<string> seatNumbers, int flightId);
        Task<bool> ValidateSeatsForFareTypeAsync(List<string> seatNumbers, int flightId, int fareId);
    }
}
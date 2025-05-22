using AcmeAirlines.DTOs;
using AcmeAirlines.Models;

namespace AcmeAirlines.Services
{
    public interface IFlightService
    {
        Task<List<City>> GetAllCitiesAsync();
        Task<List<FlightResultDto>> SearchFlightsAsync(FlightSearchDto searchDto);
        Task<FlightResultDto> GetFlightDetailsAsync(int flightId);
    }
}
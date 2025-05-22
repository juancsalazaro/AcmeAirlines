using AcmeAirlines.DTOs;

namespace AcmeAirlines.Services
{
    public interface IAdditionalServicesService
    {
        Task<AdditionalServicesDto> GetAvailableServicesAsync(int flightId, int fareId);
        Task<decimal> CalculateServicesCostAsync(SelectedServicesDto selectedServices);
        Task<bool> ValidateServicesAvailabilityAsync(SelectedServicesDto selectedServices);
    }
}
using AcmeAirlines.DTOs;
using AcmeAirlines.Models;

namespace AcmeAirlines.Services
{
    public interface IPassengerService
    {
        // Métodos para gestión de pasajeros
        Task<bool> ValidatePassengerInformationAsync(PassengerDto passenger);
        Task<List<string>> GetDocumentTypesAsync();
        Task<List<string>> GetNationalitiesAsync();

        // Métodos para validaciones específicas
        Task<bool> ValidateUnaccompaniedMinorAsync(PassengerDto passenger);
        Task<bool> ValidatePassengerLimitsAsync(int flightId, int passengerCount);
    }
}
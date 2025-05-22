using AcmeAirlines.DTOs;
using AcmeAirlines.Models;

namespace AcmeAirlines.Services
{
    public interface IPassengerService
    {
        // M�todos para gesti�n de pasajeros
        Task<bool> ValidatePassengerInformationAsync(PassengerDto passenger);
        Task<List<string>> GetDocumentTypesAsync();
        Task<List<string>> GetNationalitiesAsync();

        // M�todos para validaciones espec�ficas
        Task<bool> ValidateUnaccompaniedMinorAsync(PassengerDto passenger);
        Task<bool> ValidatePassengerLimitsAsync(int flightId, int passengerCount);
    }
}
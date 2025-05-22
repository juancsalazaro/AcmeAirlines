using Microsoft.EntityFrameworkCore;
using AcmeAirlines.Data;
using AcmeAirlines.DTOs;
using AcmeAirlines.Models;

namespace AcmeAirlines.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly AcmeAirlinesContext _context;

        public PassengerService(AcmeAirlinesContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidatePassengerInformationAsync(PassengerDto passenger)
        {
            // Validación básica de edad
            if (passenger.DateOfBirth > DateTime.Today)
            {
                return false; // Fecha de nacimiento futura no válida
            }

            // Validar que un menor no viaje solo si es menor de 12 años
            if (passenger.IsMinor && passenger.Age < 12 && passenger.IsUnaccompaniedMinor)
            {
                return false; // Menores de 12 años no pueden viajar solos
            }

            // Otras validaciones específicas pueden agregarse aquí

            return true;
        }

        public async Task<List<string>> GetDocumentTypesAsync()
        {
            // Lista estática de tipos de documento
            return new List<string>
            {
                "Pasaporte",
                "DNI / Cédula de Identidad",
                "Tarjeta de Residencia",
                "Otro"
            };
        }

        public async Task<List<string>> GetNationalitiesAsync()
        {
            // Lista de algunas nacionalidades comunes en Latinoamérica
            return new List<string>
            {
                "Argentina",
                "Bolivia",
                "Brasil",
                "Chile",
                "Colombia",
                "Costa Rica",
                "Cuba",
                "Ecuador",
                "El Salvador",
                "España",
                "Estados Unidos",
                "Guatemala",
                "Honduras",
                "México",
                "Nicaragua",
                "Panamá",
                "Paraguay",
                "Perú",
                "Puerto Rico",
                "República Dominicana",
                "Uruguay",
                "Venezuela",
                "Otra"
            };
        }

        public async Task<bool> ValidateUnaccompaniedMinorAsync(PassengerDto passenger)
        {
            // Lógica para validar menores no acompañados
            // Un menor no acompañado debe tener al menos 12 años pero ser menor de 18
            if (passenger.IsUnaccompaniedMinor)
            {
                return passenger.Age >= 12 && passenger.Age < 18;
            }

            return true; // No es un menor no acompañado, no se requiere validación
        }

        public async Task<bool> ValidatePassengerLimitsAsync(int flightId, int passengerCount)
        {
            // Verificar que no se exceda el límite de pasajeros por reserva
            const int MaxPassengersPerBooking = 9;
            if (passengerCount > MaxPassengersPerBooking)
            {
                return false;
            }

            // Verificar que haya suficientes asientos disponibles en el vuelo
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null || flight.AvailableSeats < passengerCount)
            {
                return false;
            }

            return true;
        }
    }
}
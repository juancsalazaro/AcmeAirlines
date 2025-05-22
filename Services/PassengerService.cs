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
            // Validaci�n b�sica de edad
            if (passenger.DateOfBirth > DateTime.Today)
            {
                return false; // Fecha de nacimiento futura no v�lida
            }

            // Validar que un menor no viaje solo si es menor de 12 a�os
            if (passenger.IsMinor && passenger.Age < 12 && passenger.IsUnaccompaniedMinor)
            {
                return false; // Menores de 12 a�os no pueden viajar solos
            }

            // Otras validaciones espec�ficas pueden agregarse aqu�

            return true;
        }

        public async Task<List<string>> GetDocumentTypesAsync()
        {
            // Lista est�tica de tipos de documento
            return new List<string>
            {
                "Pasaporte",
                "DNI / C�dula de Identidad",
                "Tarjeta de Residencia",
                "Otro"
            };
        }

        public async Task<List<string>> GetNationalitiesAsync()
        {
            // Lista de algunas nacionalidades comunes en Latinoam�rica
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
                "Espa�a",
                "Estados Unidos",
                "Guatemala",
                "Honduras",
                "M�xico",
                "Nicaragua",
                "Panam�",
                "Paraguay",
                "Per�",
                "Puerto Rico",
                "Rep�blica Dominicana",
                "Uruguay",
                "Venezuela",
                "Otra"
            };
        }

        public async Task<bool> ValidateUnaccompaniedMinorAsync(PassengerDto passenger)
        {
            // L�gica para validar menores no acompa�ados
            // Un menor no acompa�ado debe tener al menos 12 a�os pero ser menor de 18
            if (passenger.IsUnaccompaniedMinor)
            {
                return passenger.Age >= 12 && passenger.Age < 18;
            }

            return true; // No es un menor no acompa�ado, no se requiere validaci�n
        }

        public async Task<bool> ValidatePassengerLimitsAsync(int flightId, int passengerCount)
        {
            // Verificar que no se exceda el l�mite de pasajeros por reserva
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
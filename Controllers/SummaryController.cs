using Microsoft.AspNetCore.Mvc;
using AcmeAirlines.DTOs;
using AcmeAirlines.Services;
using System.Text.Json;

namespace AcmeAirlines.Controllers
{
    public class SummaryController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;

        public SummaryController(IBookingService bookingService, IFlightService flightService)
        {
            _bookingService = bookingService;
            _flightService = flightService;
        }

        public async Task<IActionResult> Index()
        {
            // Verificar que se hayan completado todos los pasos anteriores
            if (!TempData.ContainsKey("SelectedFlightId") ||
                !TempData.ContainsKey("SelectedFareId") ||
                !TempData.ContainsKey("PassengerInfo") ||
                !TempData.ContainsKey("SeatSelection"))
            {
                return RedirectToAction("Index", "Flight");
            }

            // Preservar los datos en TempData
            TempData.Keep("SelectedFlightId");
            TempData.Keep("SelectedFareId");
            TempData.Keep("SelectedFareName");
            TempData.Keep("SelectedFarePrice");
            TempData.Keep("PassengerInfo");
            TempData.Keep("SeatSelection");

            if (TempData.ContainsKey("SelectedServices"))
            {
                TempData.Keep("SelectedServices");
            }

            // Obtener información necesaria
            int flightId = (int)TempData["SelectedFlightId"];
            int fareId = (int)TempData["SelectedFareId"];

            // Deserializar datos de pasos anteriores
            var passengerInfoJson = TempData["PassengerInfo"].ToString();
            var passengers = JsonSerializer.Deserialize<PassengerListDto>(passengerInfoJson);

            var seatSelectionJson = TempData["SeatSelection"].ToString();
            var seatSelection = JsonSerializer.Deserialize<SeatSelectionDto>(seatSelectionJson);

            // Servicios adicionales (puede ser null si no se seleccionaron)
            SelectedServicesDto selectedServices = null;
            if (TempData.ContainsKey("SelectedServices") && TempData["SelectedServices"] != null)
            {
                var selectedServicesJson = TempData["SelectedServices"].ToString();
                selectedServices = JsonSerializer.Deserialize<SelectedServicesDto>(selectedServicesJson);
            }
            else
            {
                // Crear un objeto vacío si no hay servicios seleccionados
                selectedServices = new SelectedServicesDto { FlightId = flightId };
            }

            // Obtener el resumen de la reserva
            var bookingSummary = await _bookingService.GetBookingSummaryAsync(
                flightId,
                fareId,
                passengers,
                seatSelection,
                selectedServices);

            if (bookingSummary == null)
            {
                return RedirectToAction("Index", "Flight");
            }

            // Generar un código de reserva temporal (no confirmado)
            ViewBag.ReservationCode = await _bookingService.GenerateReservationCodeAsync();

            return View(bookingSummary);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm()
        {
            // Verificar que se hayan completado todos los pasos anteriores
            if (!TempData.ContainsKey("SelectedFlightId") ||
                !TempData.ContainsKey("SelectedFareId") ||
                !TempData.ContainsKey("PassengerInfo") ||
                !TempData.ContainsKey("SeatSelection"))
            {
                return RedirectToAction("Index", "Flight");
            }

            // Aquí iría la lógica para guardar la reserva en la base de datos
            // y redirigir al proceso de pago

            // Para la entrega 2, simplemente mostraremos una confirmación
            ViewBag.ReservationCode = await _bookingService.GenerateReservationCodeAsync();

            // Limpiar TempData para una nueva reserva
            TempData.Clear();

            return View("Confirmation");
        }

        public IActionResult Cancel()
        {
            // Cancelar la reserva y volver a la página inicial
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
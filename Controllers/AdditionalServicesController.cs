using Microsoft.AspNetCore.Mvc;
using AcmeAirlines.DTOs;
using AcmeAirlines.Services;
using System.Text.Json;

namespace AcmeAirlines.Controllers
{
    public class AdditionalServicesController : Controller
    {
        private readonly IAdditionalServicesService _additionalServicesService;
        private readonly IFlightService _flightService;

        public AdditionalServicesController(
            IAdditionalServicesService additionalServicesService,
            IFlightService flightService)
        {
            _additionalServicesService = additionalServicesService;
            _flightService = flightService;
        }

        public async Task<IActionResult> Index()
        {
            // Verificar que se hayan completado los pasos anteriores
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

            // Obtener información necesaria
            int flightId = (int)TempData["SelectedFlightId"];
            int fareId = (int)TempData["SelectedFareId"];

            // Obtener servicios disponibles
            var services = await _additionalServicesService.GetAvailableServicesAsync(flightId, fareId);

            // Obtener información del vuelo
            var flight = await _flightService.GetFlightDetailsAsync(flightId);

            if (flight == null)
            {
                return RedirectToAction("Index", "Flight");
            }

            // Configurar la vista
            ViewBag.FlightInfo = flight;
            ViewBag.SelectedFareName = TempData["SelectedFareName"];

            return View(services);
        }

        [HttpPost]
        public async Task<IActionResult> SelectServices(SelectedServicesDto selectedServices)
        {
            // Verificar que se hayan completado los pasos anteriores
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

            // Obtener información necesaria
            int flightId = (int)TempData["SelectedFlightId"];
            selectedServices.FlightId = flightId;

            // Validar disponibilidad de servicios
            if (!await _additionalServicesService.ValidateServicesAvailabilityAsync(selectedServices))
            {
                ModelState.AddModelError("", "Uno o más servicios seleccionados no están disponibles");

                // Recargar servicios disponibles
                int fareId = (int)TempData["SelectedFareId"];
                var services = await _additionalServicesService.GetAvailableServicesAsync(flightId, fareId);
                var flight = await _flightService.GetFlightDetailsAsync(flightId);

                ViewBag.FlightInfo = flight;
                ViewBag.SelectedFareName = TempData["SelectedFareName"];

                return View("Index", services);
            }

            // Calcular costos adicionales
            selectedServices.TotalAdditionalCost = await _additionalServicesService.CalculateServicesCostAsync(selectedServices);

            // Guardar la selección de servicios para el siguiente paso
            TempData["SelectedServices"] = JsonSerializer.Serialize(selectedServices);

            // Continuar al siguiente paso: resumen de reserva
            return RedirectToAction("Index", "Summary");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateServiceQuantity(int serviceId, string serviceType, int quantity)
        {
            // Verificar cantidad válida
            if (quantity < 0)
            {
                return BadRequest("La cantidad no puede ser negativa");
            }

            // Esta acción sería para AJAX, para actualizar dinámicamente las cantidades
            // Por ejemplo, para actualizar la cantidad de maletas adicionales

            return Json(new { success = true });
        }
    }
}
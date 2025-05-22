using Microsoft.AspNetCore.Mvc;
using AcmeAirlines.DTOs;
using AcmeAirlines.Services;
using System.Text.Json;

namespace AcmeAirlines.Controllers
{
    public class SeatController : Controller
    {
        private readonly ISeatService _seatService;
        private readonly IFlightService _flightService;

        public SeatController(ISeatService seatService, IFlightService flightService)
        {
            _seatService = seatService;
            _flightService = flightService;
        }

        public async Task<IActionResult> Index()
        {
            System.Diagnostics.Debug.WriteLine("Entrada a SeatController.Index");

            // Verificar que se haya completado el paso anterior
            if (!TempData.ContainsKey("SelectedFlightId") || !TempData.ContainsKey("PassengerInfo"))
            {
                System.Diagnostics.Debug.WriteLine("Falta información requerida en TempData. Redirigiendo a Flight/Index");
                return RedirectToAction("Index", "Flight");
            }

            System.Diagnostics.Debug.WriteLine("TempData contiene la información necesaria");

            // Preservar los datos en TempData
            TempData.Keep("SelectedFlightId");
            TempData.Keep("SelectedFareId");
            TempData.Keep("SelectedFareName");
            TempData.Keep("SelectedFarePrice");
            TempData.Keep("PassengerInfo");

            // Obtener información del vuelo
            int flightId = (int)TempData["SelectedFlightId"];
            System.Diagnostics.Debug.WriteLine($"FlightId: {flightId}");

            var flight = await _flightService.GetFlightDetailsAsync(flightId);

            if (flight == null)
            {
                System.Diagnostics.Debug.WriteLine("No se encontró el vuelo. Redirigiendo a Flight/Index");
                return RedirectToAction("Index", "Flight");
            }

            // Obtener información de pasajeros
            var passengerInfoJson = TempData["PassengerInfo"].ToString();
            System.Diagnostics.Debug.WriteLine($"PassengerInfo JSON: {passengerInfoJson.Substring(0, Math.Min(100, passengerInfoJson.Length))}...");

            var passengerList = JsonSerializer.Deserialize<PassengerListDto>(passengerInfoJson);
            System.Diagnostics.Debug.WriteLine($"Pasajeros deserializados: {passengerList.TotalPassengers}");

            // Obtener el mapa de asientos
            var seatMap = await _seatService.GetSeatMapAsync(flightId);
            System.Diagnostics.Debug.WriteLine($"Mapa de asientos obtenido: {seatMap.Seats.Count} asientos");

            // Configurar la vista
            ViewBag.FlightInfo = flight;
            ViewBag.PassengerCount = passengerList.TotalPassengers;
            ViewBag.SelectedFareName = TempData["SelectedFareName"];

            System.Diagnostics.Debug.WriteLine("Retornando vista con mapa de asientos");
            return View(seatMap);
        }

        [HttpPost]
        public async Task<IActionResult> SelectSeats(SeatSelectionDto seatSelection)
        {
            // Verificar que se haya completado el paso anterior
            if (!TempData.ContainsKey("SelectedFlightId") || !TempData.ContainsKey("SelectedFareId") || !TempData.ContainsKey("PassengerInfo"))
            {
                return RedirectToAction("Index", "Flight");
            }

            // Preservar los datos en TempData
            TempData.Keep("SelectedFlightId");
            TempData.Keep("SelectedFareId");
            TempData.Keep("SelectedFareName");
            TempData.Keep("SelectedFarePrice");
            TempData.Keep("PassengerInfo");

            // Obtener información necesaria
            int flightId = (int)TempData["SelectedFlightId"];
            int fareId = (int)TempData["SelectedFareId"];
            var passengerInfoJson = TempData["PassengerInfo"].ToString();
            var passengerList = JsonSerializer.Deserialize<PassengerListDto>(passengerInfoJson);

            // Validar número de asientos seleccionados
            if (seatSelection.SelectedSeats.Count != passengerList.TotalPassengers)
            {
                ModelState.AddModelError("", $"Debe seleccionar exactamente {passengerList.TotalPassengers} asientos");

                // Recargar el mapa de asientos
                var seatMap = await _seatService.GetSeatMapAsync(flightId);
                var flight = await _flightService.GetFlightDetailsAsync(flightId);

                ViewBag.FlightInfo = flight;
                ViewBag.PassengerCount = passengerList.TotalPassengers;
                ViewBag.SelectedFareName = TempData["SelectedFareName"];

                return View("Index", seatMap);
            }

            // Validar que todos los asientos estén disponibles
            foreach (var seat in seatSelection.SelectedSeats)
            {
                if (!await _seatService.IsSeatAvailableAsync(flightId, seat))
                {
                    ModelState.AddModelError("", $"El asiento {seat} ya no está disponible. Por favor, seleccione otro asiento.");

                    // Recargar el mapa de asientos
                    var seatMap = await _seatService.GetSeatMapAsync(flightId);
                    var flight = await _flightService.GetFlightDetailsAsync(flightId);

                    ViewBag.FlightInfo = flight;
                    ViewBag.PassengerCount = passengerList.TotalPassengers;
                    ViewBag.SelectedFareName = TempData["SelectedFareName"];

                    return View("Index", seatMap);
                }
            }

            // Validar que los asientos sean compatibles con el tipo de tarifa
            if (!await _seatService.ValidateSeatsForFareTypeAsync(seatSelection.SelectedSeats, flightId, fareId))
            {
                ModelState.AddModelError("", "Uno o más asientos seleccionados no están disponibles para el tipo de tarifa seleccionada");

                // Recargar el mapa de asientos
                var seatMap = await _seatService.GetSeatMapAsync(flightId);
                var flight = await _flightService.GetFlightDetailsAsync(flightId);

                ViewBag.FlightInfo = flight;
                ViewBag.PassengerCount = passengerList.TotalPassengers;
                ViewBag.SelectedFareName = TempData["SelectedFareName"];

                return View("Index", seatMap);
            }

            // Calcular cargos extra por asientos
            seatSelection.FlightId = flightId;
            seatSelection.TotalExtraCharge = await _seatService.CalculateSeatExtraChargesAsync(seatSelection.SelectedSeats, flightId);

            // Guardar la selección de asientos para el siguiente paso
            TempData["SeatSelection"] = JsonSerializer.Serialize(seatSelection);

            // Continuar al siguiente paso: servicios adicionales
            return RedirectToAction("Index", "AdditionalServices");
        }

        [HttpGet]
        public async Task<IActionResult> GetSeatInfo(int flightId, string seatNumber)
        {
            // Obtener información del asiento para mostrar en un tooltip o modal
            var seatMap = await _seatService.GetSeatMapAsync(flightId);

            if (seatMap == null)
            {
                return NotFound();
            }

            var seat = seatMap.Seats.FirstOrDefault(s => s.SeatNumber == seatNumber);

            if (seat == null)
            {
                return NotFound();
            }

            return Json(new
            {
                seatNumber = seat.SeatNumber,
                seatClass = seat.SeatClass,
                isWindow = seat.IsWindow,
                isAisle = seat.IsAisle,
                isEmergencyExit = seat.IsEmergencyExit,
                extraCharge = seat.ExtraCharge,
                isAvailable = seat.IsAvailable
            });
        }
    }
}
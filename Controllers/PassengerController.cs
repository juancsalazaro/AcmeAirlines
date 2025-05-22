using Microsoft.AspNetCore.Mvc;
using AcmeAirlines.DTOs;
using AcmeAirlines.Services;

namespace AcmeAirlines.Controllers
{
    public class PassengerController : Controller
    {
        private readonly IPassengerService _passengerService;
        private readonly IFlightService _flightService;

        public PassengerController(IPassengerService passengerService, IFlightService flightService)
        {
            _passengerService = passengerService;
            _flightService = flightService;
        }

        public async Task<IActionResult> Index()
        {
            // Verificar que se haya seleccionado un vuelo y tarifa
            if (!TempData.ContainsKey("SelectedFlightId") || !TempData.ContainsKey("SelectedFareId"))
            {
                return RedirectToAction("Index", "Flight");
            }

            // Preservar los datos en TempData para mantenerlos disponibles
            TempData.Keep("SelectedFlightId");
            TempData.Keep("SelectedFareId");
            TempData.Keep("SelectedFareName");
            TempData.Keep("SelectedFarePrice");
            TempData.Keep("SearchPassengers");

            // Obtener datos del vuelo para mostrar en la vista
            int flightId = (int)TempData["SelectedFlightId"];
            var flight = await _flightService.GetFlightDetailsAsync(flightId);

            if (flight == null)
            {
                return RedirectToAction("Index", "Flight");
            }

            // Configurar la vista con datos necesarios
            ViewBag.FlightInfo = flight;
            ViewBag.SelectedFareName = TempData["SelectedFareName"];
            ViewBag.SelectedFarePrice = TempData["SelectedFarePrice"];
            ViewBag.DocumentTypes = await _passengerService.GetDocumentTypesAsync();
            ViewBag.Nationalities = await _passengerService.GetNationalitiesAsync();

            // Número de pasajeros para la reserva
            int passengerCount = 1; // Valor por defecto
            if (TempData.ContainsKey("SearchPassengers"))
            {
                passengerCount = (int)TempData["SearchPassengers"];
            }
            ViewBag.PassengerCount = passengerCount;

            // Inicializar el modelo de pasajeros
            var passengerListDto = new PassengerListDto
            {
                MainPassenger = new PassengerDto(),
                EmergencyContact = new ContactInfoDto() // Inicializar el contacto de emergencia aunque no sea necesario
            };

            return View(passengerListDto);
        }

        [HttpPost]
        public async Task<IActionResult> SavePassengers(PassengerListDto passengerList)
        {
            // Agregar mensaje de depuración
            System.Diagnostics.Debug.WriteLine("Procesando formulario de pasajeros...");

            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("Modelo inválido. Errores: " + string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));

                // Recargar datos necesarios para la vista
                ViewBag.DocumentTypes = await _passengerService.GetDocumentTypesAsync();
                ViewBag.Nationalities = await _passengerService.GetNationalitiesAsync();

                // Obtener datos del vuelo nuevamente
                if (TempData.ContainsKey("SelectedFlightId"))
                {
                    int flightId = (int)TempData["SelectedFlightId"];
                    TempData.Keep("SelectedFlightId");
                    TempData.Keep("SelectedFareId");
                    TempData.Keep("SelectedFareName");
                    TempData.Keep("SelectedFarePrice");
                    TempData.Keep("SearchPassengers");

                    var flight = await _flightService.GetFlightDetailsAsync(flightId);
                    ViewBag.FlightInfo = flight;
                    ViewBag.SelectedFareName = TempData["SelectedFareName"];
                    ViewBag.SelectedFarePrice = TempData["SelectedFarePrice"];
                    ViewBag.PassengerCount = TempData.ContainsKey("SearchPassengers") ? (int)TempData["SearchPassengers"] : 1;
                }

                return View("Index", passengerList);
            }

            // Validar la información de los pasajeros
            if (!await _passengerService.ValidatePassengerInformationAsync(passengerList.MainPassenger))
            {
                ModelState.AddModelError("", "La información del pasajero principal no es válida");
                ViewBag.DocumentTypes = await _passengerService.GetDocumentTypesAsync();
                ViewBag.Nationalities = await _passengerService.GetNationalitiesAsync();

                if (TempData.ContainsKey("SelectedFlightId"))
                {
                    int flightId = (int)TempData["SelectedFlightId"];
                    TempData.Keep("SelectedFlightId");
                    TempData.Keep("SelectedFareId");
                    TempData.Keep("SelectedFareName");
                    TempData.Keep("SelectedFarePrice");
                    TempData.Keep("SearchPassengers");

                    var flight = await _flightService.GetFlightDetailsAsync(flightId);
                    ViewBag.FlightInfo = flight;
                    ViewBag.SelectedFareName = TempData["SelectedFareName"];
                    ViewBag.SelectedFarePrice = TempData["SelectedFarePrice"];
                    ViewBag.PassengerCount = TempData.ContainsKey("SearchPassengers") ? (int)TempData["SearchPassengers"] : 1;
                }

                return View("Index", passengerList);
            }

            // Validar pasajeros adicionales
            foreach (var passenger in passengerList.AdditionalPassengers)
            {
                if (!await _passengerService.ValidatePassengerInformationAsync(passenger))
                {
                    ModelState.AddModelError("", "La información de uno o más pasajeros adicionales no es válida");
                    ViewBag.DocumentTypes = await _passengerService.GetDocumentTypesAsync();
                    ViewBag.Nationalities = await _passengerService.GetNationalitiesAsync();

                    if (TempData.ContainsKey("SelectedFlightId"))
                    {
                        int flightId = (int)TempData["SelectedFlightId"];
                        TempData.Keep("SelectedFlightId");
                        TempData.Keep("SelectedFareId");
                        TempData.Keep("SelectedFareName");
                        TempData.Keep("SelectedFarePrice");
                        TempData.Keep("SearchPassengers");

                        var flight = await _flightService.GetFlightDetailsAsync(flightId);
                        ViewBag.FlightInfo = flight;
                        ViewBag.SelectedFareName = TempData["SelectedFareName"];
                        ViewBag.SelectedFarePrice = TempData["SelectedFarePrice"];
                        ViewBag.PassengerCount = TempData.ContainsKey("SearchPassengers") ? (int)TempData["SearchPassengers"] : 1;
                    }

                    return View("Index", passengerList);
                }
            }

            // Validar menores no acompañados
            bool hasUnaccompaniedMinor = passengerList.MainPassenger.IsUnaccompaniedMinor ||
                                        (passengerList.AdditionalPassengers != null &&
                                         passengerList.AdditionalPassengers.Any(p => p.IsUnaccompaniedMinor));

            // Solo validar contacto de emergencia si hay menores no acompañados
            if (hasUnaccompaniedMinor)
            {
                if (passengerList.EmergencyContact == null)
                {
                    // Si no hay un objeto de contacto de emergencia, crearlo
                    passengerList.EmergencyContact = new ContactInfoDto();
                }

                if (string.IsNullOrEmpty(passengerList.EmergencyContact.Name) ||
                    string.IsNullOrEmpty(passengerList.EmergencyContact.Phone) ||
                    string.IsNullOrEmpty(passengerList.EmergencyContact.Relationship))
                {
                    ModelState.AddModelError("", "Para menores no acompañados es obligatorio proporcionar un contacto de emergencia completo (nombre, relación y teléfono)");
                    ViewBag.DocumentTypes = await _passengerService.GetDocumentTypesAsync();
                    ViewBag.Nationalities = await _passengerService.GetNationalitiesAsync();

                    if (TempData.ContainsKey("SelectedFlightId"))
                    {
                        int flightId = (int)TempData["SelectedFlightId"];
                        TempData.Keep("SelectedFlightId");
                        TempData.Keep("SelectedFareId");
                        TempData.Keep("SelectedFareName");
                        TempData.Keep("SelectedFarePrice");
                        TempData.Keep("SearchPassengers");

                        var flight = await _flightService.GetFlightDetailsAsync(flightId);
                        ViewBag.FlightInfo = flight;
                        ViewBag.SelectedFareName = TempData["SelectedFareName"];
                        ViewBag.SelectedFarePrice = TempData["SelectedFarePrice"];
                        ViewBag.PassengerCount = TempData.ContainsKey("SearchPassengers") ? (int)TempData["SearchPassengers"] : 1;
                    }

                    return View("Index", passengerList);
                }
            }
            else
            {
                // Si no hay menores no acompañados, el contacto de emergencia no es necesario
                // Asegurarse de que se sigue procesando incluso si los campos están vacíos
                if (passengerList.EmergencyContact != null)
                {
                    // Limpiar datos de contacto de emergencia para evitar validaciones innecesarias
                    passengerList.EmergencyContact = null;
                }
            }

            // Almacenar la información de los pasajeros para los siguientes pasos
            string passengerInfoJson = System.Text.Json.JsonSerializer.Serialize(passengerList);
            TempData["PassengerInfo"] = passengerInfoJson;

            // Preservar otros datos en TempData
            TempData.Keep("SelectedFlightId");
            TempData.Keep("SelectedFareId");
            TempData.Keep("SelectedFareName");
            TempData.Keep("SelectedFarePrice");

            System.Diagnostics.Debug.WriteLine("Información de pasajeros guardada correctamente. Redirigiendo a selección de asientos...");

            // Continuar al siguiente paso: selección de asientos
            return RedirectToAction("Index", "Seat");
        }

        public IActionResult AddPassenger()
        {
            return PartialView("_PassengerForm", new PassengerDto());
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePassenger(PassengerDto passenger)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            bool isValid = await _passengerService.ValidatePassengerInformationAsync(passenger);
            return Json(new { success = isValid });
        }
    }
}
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

            // N�mero de pasajeros para la reserva
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
            // Agregar mensaje de depuraci�n
            System.Diagnostics.Debug.WriteLine("Procesando formulario de pasajeros...");

            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("Modelo inv�lido. Errores: " + string.Join(", ", ModelState.Values
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

            // Validar la informaci�n de los pasajeros
            if (!await _passengerService.ValidatePassengerInformationAsync(passengerList.MainPassenger))
            {
                ModelState.AddModelError("", "La informaci�n del pasajero principal no es v�lida");
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
                    ModelState.AddModelError("", "La informaci�n de uno o m�s pasajeros adicionales no es v�lida");
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

            // Validar menores no acompa�ados
            bool hasUnaccompaniedMinor = passengerList.MainPassenger.IsUnaccompaniedMinor ||
                                        (passengerList.AdditionalPassengers != null &&
                                         passengerList.AdditionalPassengers.Any(p => p.IsUnaccompaniedMinor));

            // Solo validar contacto de emergencia si hay menores no acompa�ados
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
                    ModelState.AddModelError("", "Para menores no acompa�ados es obligatorio proporcionar un contacto de emergencia completo (nombre, relaci�n y tel�fono)");
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
                // Si no hay menores no acompa�ados, el contacto de emergencia no es necesario
                // Asegurarse de que se sigue procesando incluso si los campos est�n vac�os
                if (passengerList.EmergencyContact != null)
                {
                    // Limpiar datos de contacto de emergencia para evitar validaciones innecesarias
                    passengerList.EmergencyContact = null;
                }
            }

            // Almacenar la informaci�n de los pasajeros para los siguientes pasos
            string passengerInfoJson = System.Text.Json.JsonSerializer.Serialize(passengerList);
            TempData["PassengerInfo"] = passengerInfoJson;

            // Preservar otros datos en TempData
            TempData.Keep("SelectedFlightId");
            TempData.Keep("SelectedFareId");
            TempData.Keep("SelectedFareName");
            TempData.Keep("SelectedFarePrice");

            System.Diagnostics.Debug.WriteLine("Informaci�n de pasajeros guardada correctamente. Redirigiendo a selecci�n de asientos...");

            // Continuar al siguiente paso: selecci�n de asientos
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
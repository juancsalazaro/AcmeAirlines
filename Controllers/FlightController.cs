using Microsoft.AspNetCore.Mvc;
using AcmeAirlines.DTOs;
using AcmeAirlines.Services;

namespace AcmeAirlines.Controllers
{
    public class FlightController : Controller
    {
        private readonly IFlightService _flightService;

        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Cities = await _flightService.GetAllCitiesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(FlightSearchDto searchDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = await _flightService.GetAllCitiesAsync();
                return View("Index", searchDto);
            }

            try
            {
                var searchDtoUtc = new FlightSearchDto
                {
                    OriginCityId = searchDto.OriginCityId,
                    DestinationCityId = searchDto.DestinationCityId,
                    DepartureDate = DateTime.SpecifyKind(searchDto.DepartureDate.Date, DateTimeKind.Utc),
                    ReturnDate = searchDto.ReturnDate.HasValue
                        ? DateTime.SpecifyKind(searchDto.ReturnDate.Value.Date, DateTimeKind.Utc)
                        : null,
                    Passengers = searchDto.Passengers
                };

                var flights = await _flightService.SearchFlightsAsync(searchDtoUtc);

                if (flights.Count == 0)
                {
                    ViewBag.ErrorMessage = "No se encontraron vuelos disponibles para la fecha y ruta seleccionadas.";
                    ViewBag.Cities = await _flightService.GetAllCitiesAsync();
                    return View("Index", searchDto);
                }

                TempData["SearchOriginId"] = searchDto.OriginCityId;
                TempData["SearchDestinationId"] = searchDto.DestinationCityId;
                TempData["SearchDepartureDate"] = searchDto.DepartureDate.ToString("yyyy-MM-dd");
                TempData["SearchPassengers"] = searchDto.Passengers;

                return View("SearchResults", flights);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Cities = await _flightService.GetAllCitiesAsync();
                return View("Index", searchDto);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var flight = await _flightService.GetFlightDetailsAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        public async Task<IActionResult> SelectFare(int flightId, int fareId)
        {
            var flight = await _flightService.GetFlightDetailsAsync(flightId);

            if (flight == null)
            {
                return NotFound();
            }

            var selectedFare = flight.Fares.FirstOrDefault(f => f.FareId == fareId);

            if (selectedFare == null)
            {
                return NotFound();
            }

            TempData["SelectedFlightId"] = flightId;
            TempData["SelectedFareId"] = fareId;
            TempData["SelectedFareName"] = selectedFare.Name;
            TempData["SelectedFarePrice"] = selectedFare.Price.ToString();

            return RedirectToAction("Index", "Passenger");
        }
    }
}
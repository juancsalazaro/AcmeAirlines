using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AcmeAirlines.Models;
using AcmeAirlines.Services;

namespace AcmeAirlines.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFlightService _flightService;

        public HomeController(ILogger<HomeController> logger, IFlightService flightService)
        {
            _logger = logger;
            _flightService = flightService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Cities = await _flightService.GetAllCitiesAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
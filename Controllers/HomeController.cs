using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using week1.Models;

namespace week1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }
            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("DoctorDashboard");
            }
            if (User.IsInRole("Receptionist"))
            {
                return RedirectToAction("ReceptionistDashboard");
            }

            // Fallback: if user somehow authenticated with an invalid/different role, redirect to unauthorized
            return RedirectToAction("UnauthorizedPage", "Account");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult DoctorDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Receptionist")]
        public IActionResult ReceptionistDashboard()
        {
            return View();
        }

        [Authorize]
        public IActionResult AnalyticsDashboard()
        {
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

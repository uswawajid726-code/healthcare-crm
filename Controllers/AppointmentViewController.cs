using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
    /// <summary>
    /// MVC controller for rendering Appointment management Razor Views.
    /// Access strictly enforced via role-based authorization.
    /// </summary>
    [Authorize]
    public class AppointmentViewController : Controller
    {
        /// <summary>
        /// Renders the Appointment List and Management view.
        /// </summary>
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Renders the form to schedule a new appointment.
        /// </summary>
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Renders the form to modify an existing appointment.
        /// </summary>
        /// <param name="id">Appointment ID.</param>
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.AppointmentId = id;
            return View();
        }

        /// <summary>
        /// Renders the detailed appointment profile view.
        /// </summary>
        /// <param name="id">Appointment ID.</param>
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewBag.AppointmentId = id;
            return View();
        }
    }
}

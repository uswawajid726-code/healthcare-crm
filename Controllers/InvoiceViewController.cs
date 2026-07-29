using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
    /// <summary>
    /// MVC controller for rendering Invoice and Billing Views.
    /// Restricted to authorized staff roles.
    /// </summary>
    [Authorize]
    public class InvoiceViewController : Controller
    {
        /// <summary>
        /// Renders the main Invoice and Billing registry view.
        /// </summary>
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Renders the detailed invoice view for a given invoice ID.
        /// </summary>
        /// <param name="id">The unique ID of the invoice.</param>
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewBag.InvoiceId = id;
            return View();
        }

        /// <summary>
        /// Renders the form to create a new invoice.
        /// </summary>
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
    }
}
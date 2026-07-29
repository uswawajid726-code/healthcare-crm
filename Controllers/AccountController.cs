using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
    /// <summary>
    /// MVC controller for user account authentication views (Login, Register, Logout, Unauthorized).
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Renders the user login view.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Renders the user registration view.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Renders the user logout confirmation / session clearance view.
        /// </summary>
        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        /// <summary>
        /// Renders the access denied / unauthorized page.
        /// </summary>
        [HttpGet]
        public IActionResult UnauthorizedPage()
        {
            return View();
        }
    }
}

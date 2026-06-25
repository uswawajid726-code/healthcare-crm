using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // GET: Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        // GET: Account/UnauthorizedPage
        [HttpGet]
        public IActionResult UnauthorizedPage()
        {
            return View();
        }
    }
}

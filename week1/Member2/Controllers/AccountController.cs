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
            // Just returns view or we can redirect to login. Since we clear token client-side,
            // we will let the view clear it and redirect.
            return View();
        }
    }
}

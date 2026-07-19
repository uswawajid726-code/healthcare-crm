using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
	[Authorize]
	public class InvoiceViewController : Controller
	{
		// GET: InvoiceView
		[Authorize(Roles = "Admin,Doctor,Receptionist")]
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		// GET: InvoiceView/Details/5
		[Authorize(Roles = "Admin,Doctor,Receptionist")]
		[HttpGet]
		public IActionResult Details(int id)
		{
			ViewBag.InvoiceId = id;
			return View();
		}
        // GET: InvoiceView/Create
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
    }
}
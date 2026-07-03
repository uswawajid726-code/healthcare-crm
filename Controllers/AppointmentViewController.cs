using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
    [Authorize]
    public class AppointmentViewController : Controller
    {
        // GET: AppointmentView
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: AppointmentView/Create
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // GET: AppointmentView/Edit/5
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.AppointmentId = id;
            return View();
        }

        // GET: AppointmentView/Details/5
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewBag.AppointmentId = id;
            return View();
        }
    }
}

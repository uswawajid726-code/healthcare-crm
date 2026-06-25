using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
    /// <summary>
    /// MVC controller for rendering Patient Views. 
    /// Restricts access strictly to Admin, Doctor, and Receptionist roles.
    /// </summary>
    [Authorize(Roles = "Admin,Doctor,Receptionist")]
    public class PatientViewController : Controller
    {
        // GET: PatientView
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: PatientView/Add
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // GET: PatientView/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.PatientId = id;
            return View();
        }

        // GET: PatientView/Profile/5
        [HttpGet]
        public IActionResult Profile(int id)
        {
            ViewBag.PatientId = id;
            return View();
        }
    }
}

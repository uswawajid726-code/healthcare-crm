using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
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
    }
}

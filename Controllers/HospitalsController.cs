using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;

namespace week1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HospitalsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/hospitals
        [HttpGet]
        public async Task<IActionResult> GetHospitals()
        {
            var hospitals = await _context.Hospitals.ToListAsync();

            return Ok(hospitals);
        }
    }
}
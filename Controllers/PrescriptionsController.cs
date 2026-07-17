using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PrescriptionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/prescriptions?appointmentId=1
        [HttpGet]
        public async Task<IActionResult> GetPrescriptions([FromQuery] int? appointmentId)
        {
            var prescriptions = _context.Prescriptions
                .Include(p => p.Appointment)
                .AsQueryable();

            if (appointmentId.HasValue)
            {
                prescriptions = prescriptions.Where(p => p.AppointmentId == appointmentId.Value);
            }

            return Ok(await prescriptions.ToListAsync());
        }

        // POST: api/prescriptions
        [HttpPost]
        public async Task<IActionResult> CreatePrescription(Prescription prescription)
        {
            prescription.CreatedAt = DateTime.UtcNow;

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(prescription);
        }
    }
}
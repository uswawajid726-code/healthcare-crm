using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;

namespace week1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RemindersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reminders
        [HttpGet]
        public async Task<IActionResult> GetReminders()
        {
            var now = DateTime.UtcNow;
            var next24Hours = now.AddHours(24);

            var reminders = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.AppointmentDate >= now &&
                            a.AppointmentDate <= next24Hours &&
                            a.Status == "Scheduled")
                .ToListAsync();

            return Ok(reminders);
        }
    }
}
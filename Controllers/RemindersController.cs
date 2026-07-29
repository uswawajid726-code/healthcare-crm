using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;

namespace week1.Controllers
{
    /// <summary>
    /// API controller for appointment reminder notifications.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RemindersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves appointments scheduled within the next 24 hours requiring reminder dispatches.
        /// </summary>
        /// <returns>A list of upcoming appointments for reminders.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetReminders()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving appointment reminders: {ex.Message}"
                });
            }
        }
    }
}
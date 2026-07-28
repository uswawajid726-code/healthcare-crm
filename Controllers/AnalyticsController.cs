using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;

namespace week1.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/analytics/patients
        [HttpGet("patients")]
        public async Task<IActionResult> GetPatientsAnalytics()
        {
            var totalPatients = await _context.Patients.CountAsync();

            return Ok(new
            {
                success = true,
                totalPatients = totalPatients
            });
        }


        // GET: api/analytics/appointments
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentsAnalytics()
        {
            var totalAppointments = await _context.Appointments.CountAsync();

            var completedAppointments = await _context.Appointments
                .Where(a => a.Status == "Completed")
                .CountAsync();

            return Ok(new
            {
                success = true,
                totalAppointments = totalAppointments,
                completedAppointments = completedAppointments
            });
        }


        // GET: api/analytics/doctors
        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctorsAnalytics()
        {
            var totalDoctors = await _context.Users
                .Where(u => u.Role == "Doctor")
                .CountAsync();

            return Ok(new
            {
                success = true,
                totalDoctors = totalDoctors
            });
        }
    }
}
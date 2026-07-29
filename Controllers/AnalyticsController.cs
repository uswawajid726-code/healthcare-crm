<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;

namespace week1.Controllers
{
=======
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using week1.Models;
using week1.Services;

namespace week1.Controllers
{
    /// <summary>
    /// API controller providing healthcare analytics and performance metrics.
    /// </summary>
    [Authorize]
>>>>>>> 95c392d (final fixes and project polish)
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
<<<<<<< HEAD
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
=======
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Retrieves patient demographics and registration analytics.
        /// </summary>
        /// <returns>Total patients, new patients this month, and gender distribution.</returns>
        [HttpGet("patients")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetPatientAnalytics()
        {
            try
            {
                var data = await _analyticsService.GetPatientAnalyticsAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Patient analytics retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving patient analytics: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Retrieves appointment volume trends over the last 30 days.
        /// </summary>
        /// <returns>Daily appointment counts for the last 30 days and summary metrics.</returns>
        [HttpGet("appointments")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetAppointmentAnalytics()
        {
            try
            {
                var data = await _analyticsService.GetAppointmentAnalyticsAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Appointment analytics retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving appointment analytics: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Retrieves doctor workload analytics for the current month.
        /// </summary>
        /// <returns>Appointment counts per doctor for the current month.</returns>
        [HttpGet("doctors")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetDoctorAnalytics()
        {
            try
            {
                var data = await _analyticsService.GetDoctorAnalyticsAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Doctor analytics retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving doctor analytics: {ex.Message}"
                });
            }
        }
    }
}
>>>>>>> 95c392d (final fixes and project polish)

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Controllers
{
    /// <summary>
    /// API controller for managing patient prescription records.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PrescriptionsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves prescriptions, optionally filtered by appointment ID.
        /// </summary>
        /// <param name="appointmentId">Optional appointment ID filter.</param>
        /// <returns>A list of prescription records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetPrescriptions([FromQuery] int? appointmentId)
        {
            try
            {
                var query = _context.Prescriptions
                    .Include(p => p.Appointment)
                    .AsQueryable();

                if (appointmentId.HasValue)
                {
                    query = query.Where(p => p.AppointmentId == appointmentId.Value);
                }

                var prescriptions = await query.ToListAsync();

                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving prescriptions: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Creates a new prescription record.
        /// </summary>
        /// <param name="prescription">Prescription payload.</param>
        /// <returns>The created prescription entity.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        [ProducesResponseType(typeof(ApiResponse), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> CreatePrescription([FromBody] Prescription prescription)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid prescription payload."
                    });
                }

                prescription.CreatedAt = DateTime.UtcNow;

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();

                return StatusCode(201, prescription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while creating prescription: {ex.Message}"
                });
            }
        }
    }
}
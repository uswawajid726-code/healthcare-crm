using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using week1.Models;
using week1.Services;

namespace week1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<IActionResult> GetAppointments([FromQuery] string? search = null, [FromQuery] string? status = null, [FromQuery] DateTime? date = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                int? doctorId = null;
                if (userRole == "Doctor")
                {
                    doctorId = userId;
                }

                var appointments = await _appointmentService.GetAppointmentsAsync(search, status, date, doctorId);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Appointments retrieved successfully.",
                    Data = appointments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving appointments: {ex.Message}"
                });
            }
        }

        // GET: api/appointments/doctors
        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            try
            {
                var doctors = await _appointmentService.GetDoctorsAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Doctors retrieved successfully.",
                    Data = doctors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving doctors: {ex.Message}"
                });
            }
        }

        // GET: api/appointments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Appointment with ID {id} not found."
                    });
                }

                if (userRole == "Doctor" && appointment.DoctorId != userId)
                {
                    return StatusCode(403, new ApiResponse
                    {
                        Success = false,
                        Message = "You are not authorized to view this appointment."
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Appointment details retrieved successfully.",
                    Data = appointment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving appointment details: {ex.Message}"
                });
            }
        }

        // POST: api/appointments
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid appointment data model."
                });
            }

            try
            {
                // Double booking validation
                var isAvailable = await _appointmentService.IsDoctorAvailableAsync(
                    appointment.DoctorId, 
                    appointment.AppointmentDate, 
                    appointment.AppointmentTime);

                if (!isAvailable)
                {
                    return StatusCode(409, new ApiResponse
                    {
                        Success = false,
                        Message = "This doctor already has an appointment at the selected date and time."
                    });
                }

                var success = await _appointmentService.AddAppointmentAsync(appointment);
                if (!success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Failed to create appointment. Verify patient exists and selected doctor is active."
                    });
                }

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, new ApiResponse
                {
                    Success = true,
                    Message = "Appointment created successfully.",
                    Data = appointment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while adding the appointment: {ex.Message}"
                });
            }
        }

        // PUT: api/appointments/{id}
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Appointment ID mismatch."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid appointment data model."
                });
            }

            try
            {
                // Double booking validation (excluding current appointment being updated)
                var isAvailable = await _appointmentService.IsDoctorAvailableAsync(
                    appointment.DoctorId, 
                    appointment.AppointmentDate, 
                    appointment.AppointmentTime, 
                    appointment.Id);

                if (!isAvailable)
                {
                    return StatusCode(409, new ApiResponse
                    {
                        Success = false,
                        Message = "This doctor already has an appointment at the selected date and time."
                    });
                }

                var success = await _appointmentService.UpdateAppointmentAsync(appointment);
                if (!success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Failed to update appointment. Verify appointment exists, patient exists, and selected doctor is active."
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Appointment details updated successfully.",
                    Data = appointment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating the appointment: {ex.Message}"
                });
            }
        }

        // DELETE: api/appointments/{id}
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                var success = await _appointmentService.DeleteAppointmentAsync(id);
                if (!success)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Appointment with ID {id} not found or could not be cancelled."
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Appointment record deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while deleting the appointment: {ex.Message}"
                });
            }
        }
    }
}

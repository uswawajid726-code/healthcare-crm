using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Controllers
{
    /// <summary>
    /// API controller for managing patient emergency contacts.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyContactsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmergencyContactsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all registered emergency contacts.
        /// </summary>
        /// <returns>A list of emergency contacts.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetEmergencyContacts()
        {
            try
            {
                var contacts = await _context.EmergencyContacts.ToListAsync();
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving emergency contacts: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Creates a new emergency contact record.
        /// </summary>
        /// <param name="contact">Emergency contact payload.</param>
        /// <returns>The created emergency contact.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> CreateEmergencyContact([FromBody] EmergencyContact contact)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid emergency contact payload."
                    });
                }

                contact.CreatedAt = DateTime.UtcNow;

                _context.EmergencyContacts.Add(contact);
                await _context.SaveChangesAsync();

                return StatusCode(201, contact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while creating emergency contact: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Deletes an emergency contact record by ID.
        /// </summary>
        /// <param name="id">Emergency contact ID.</param>
        /// <returns>Deletion confirmation message.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> DeleteEmergencyContact(int id)
        {
            try
            {
                var contact = await _context.EmergencyContacts.FindAsync(id);

                if (contact == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Emergency contact with ID {id} not found."
                    });
                }

                _context.EmergencyContacts.Remove(contact);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Emergency contact deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while deleting emergency contact: {ex.Message}"
                });
            }
        }
    }
}
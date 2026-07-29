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
    /// API controller for billing, invoice management, and payment processing.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoicesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of invoices, optionally filtered by status (e.g. Paid, Unpaid).
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <returns>A list of invoices with linked appointments and patients.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetInvoices([FromQuery] string? status)
        {
            try
            {
                var query = _context.Invoices
                    .Include(i => i.Appointment)
                        .ThenInclude(a => a != null ? a.Patient : null)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(i => i.Status.ToLower() == status.ToLower().Trim());
                }

                var invoices = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Invoices retrieved successfully.",
                    Data = invoices
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving invoices: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Retrieves single invoice details by ID.
        /// </summary>
        /// <param name="id">Invoice ID.</param>
        /// <returns>Invoice details with patient and appointment info.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Appointment)
                        .ThenInclude(a => a != null ? a.Patient : null)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Invoice with ID {id} not found."
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Invoice details retrieved successfully.",
                    Data = invoice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving invoice details: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Creates a new billing invoice record.
        /// </summary>
        /// <param name="invoice">Invoice payload.</param>
        /// <returns>The created invoice details.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(ApiResponse), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid invoice payload."
                    });
                }

                invoice.CreatedAt = DateTime.UtcNow;
                invoice.Status = "Unpaid";

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                return StatusCode(201, new ApiResponse
                {
                    Success = true,
                    Message = "Invoice created successfully.",
                    Data = invoice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while creating the invoice: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Marks an unpaid invoice as paid and records the payment timestamp.
        /// </summary>
        /// <param name="id">The unique ID of the invoice.</param>
        /// <returns>The updated invoice details.</returns>
        [HttpPatch("{id}/pay")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> PayInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);

                if (invoice == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Invoice with ID {id} was not found."
                    });
                }

                invoice.Status = "Paid";

                _context.Payments.Add(new Payment
                {
                    InvoiceId = id,
                    PaidAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Invoice paid successfully.",
                    Data = invoice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while processing the invoice payment: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Deletes an invoice record by ID.
        /// </summary>
        /// <param name="id">Invoice ID.</param>
        /// <returns>Deletion status.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);

                if (invoice == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Invoice with ID {id} not found."
                    });
                }

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Invoice record deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while deleting the invoice: {ex.Message}"
                });
            }
        }
    }
}
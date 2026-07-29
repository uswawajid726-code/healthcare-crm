using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;

namespace week1.Controllers
{
    /// <summary>
    /// API controller for hospital entity registry.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HospitalsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all registered hospital branches.
        /// </summary>
        /// <returns>List of hospitals.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetHospitals()
        {
            try
            {
                var hospitals = await _context.Hospitals.ToListAsync();
                return Ok(hospitals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving hospitals: {ex.Message}"
                });
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
    /// <summary>
    /// API controller for health checks and service availability monitoring.
    /// </summary>
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Performs a system health check.
        /// </summary>
        /// <returns>Health status confirmation.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public IActionResult GetHealth()
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Service is running",
                Data = new
                {
                    status = "Healthy"
                }
            });
        }
    }
}
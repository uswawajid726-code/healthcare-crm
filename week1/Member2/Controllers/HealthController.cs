dusing Microsoft.AspNetCore.Mvc;

namespace week1.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        // GET: api/health
        [HttpGet]
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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using week1.Services;

namespace week1.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid request payload."
                });
            }

            var result = await _authService.RegisterAsync(model);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = result.Message,
                Data = new
                {
                    token = result.Token,
                    user = new
                    {
                        id = result.User?.Id,
                        username = result.User?.Username,
                        email = result.User?.Email,
                        role = result.User?.Role,
                        fullName = result.User?.FullName
                    }
                }
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid request payload."
                });
            }

            var result = await _authService.LoginAsync(model);

            if (!result.Success)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = result.Message,
                Data = new
                {
                    token = result.Token,
                    user = new
                    {
                        id = result.User?.Id,
                        username = result.User?.Username,
                        email = result.User?.Email,
                        role = result.User?.Role,
                        fullName = result.User?.FullName
                    }
                }
            });
        }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

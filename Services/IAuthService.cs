using System.Threading.Tasks;
using week1.Models;

namespace week1.Services
{
    /// <summary>
    /// Contract for user registration, authentication, and JWT token issuing.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user account in the system.
        /// </summary>
        /// <param name="model">Registration payload containing credentials and profile details.</param>
        /// <returns>Authentication result detailing success or failure.</returns>
        Task<AuthResult> RegisterAsync(RegisterModel model);

        /// <summary>
        /// Authenticates user credentials and generates a signed JWT token upon success.
        /// </summary>
        /// <param name="model">Login credentials (username/email and password).</param>
        /// <returns>Authentication result containing JWT bearer token and user profile.</returns>
        Task<AuthResult> LoginAsync(LoginModel model);
    }

    /// <summary>
    /// Result payload returned by authentication operations.
    /// </summary>
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }

    /// <summary>
    /// Request DTO for user account registration.
    /// </summary>
    public class RegisterModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Patient";
        public string FullName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for user login.
    /// </summary>
    public class LoginModel
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

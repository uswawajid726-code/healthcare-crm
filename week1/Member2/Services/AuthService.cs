using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using week1.Data;
using week1.Models;

namespace week1.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResult> RegisterAsync(RegisterModel model)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(model.Username) || 
                string.IsNullOrWhiteSpace(model.Email) || 
                string.IsNullOrWhiteSpace(model.Password))
            {
                return new AuthResult { Success = false, Message = "Username, email, and password are required." };
            }

            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower()))
            {
                return new AuthResult { Success = false, Message = "Username is already taken." };
            }

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                return new AuthResult { Success = false, Message = "Email is already registered." };
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Validate role
            var role = model.Role;
            if (role != "Admin" && role != "Doctor" && role != "Patient")
            {
                role = "Patient";
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate JWT
                var token = GenerateJwtToken(user);

                return new AuthResult
                {
                    Success = true,
                    Token = token,
                    Message = "Registration successful.",
                    User = user
                };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Message = $"An error occurred during registration: {ex.Message}" };
            }
        }

        public async Task<AuthResult> LoginAsync(LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UsernameOrEmail) || string.IsNullOrWhiteSpace(model.Password))
            {
                return new AuthResult { Success = false, Message = "Username/Email and password are required." };
            }

            // Find user by username or email
            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Username.ToLower() == model.UsernameOrEmail.ToLower() || 
                u.Email.ToLower() == model.UsernameOrEmail.ToLower());

            if (user == null)
            {
                return new AuthResult { Success = false, Message = "Invalid username/email or password." };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return new AuthResult { Success = false, Message = "Invalid username/email or password." };
            }

            // Generate JWT
            var token = GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Token = token,
                Message = "Login successful.",
                User = user
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Get secret from environment variables
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                // Fallback safe key for development if env is not loaded properly
                secretKey = "DevelopmentSuperSecretSecretKeyKey12345!!!";
            }

            var key = Encoding.ASCII.GetBytes(secretKey);
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "HealthcareCRM";
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "HealthcareCRMUsers";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

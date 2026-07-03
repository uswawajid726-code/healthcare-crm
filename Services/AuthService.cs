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
            if (string.IsNullOrWhiteSpace(model.Email) || 
                string.IsNullOrWhiteSpace(model.Password) ||
                string.IsNullOrWhiteSpace(model.FullName))
            {
                return new AuthResult { Success = false, Message = "Full Name, Email, and Password are required." };
            }

            // Set username to email if not provided
            var username = string.IsNullOrWhiteSpace(model.Username) ? model.Email : model.Username;

            // Validate email format
            try
            {
                var addr = new System.Net.Mail.MailAddress(model.Email);
                if (addr.Address != model.Email)
                {
                    return new AuthResult { Success = false, Message = "Invalid email format." };
                }
            }
            catch
            {
                return new AuthResult { Success = false, Message = "Invalid email format." };
            }

            // Validate password length
            if (model.Password.Length < 6)
            {
                return new AuthResult { Success = false, Message = "Password must be at least 6 characters." };
            }

            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return new AuthResult { Success = false, Message = "Username is already taken." };
            }

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                return new AuthResult { Success = false, Message = "Email is already registered." };
            }

            // Validate role specifically for Week 2 allowed roles: Admin, Doctor, Receptionist
            var role = model.Role;
            if (role != "Admin" && role != "Doctor" && role != "Receptionist")
            {
                return new AuthResult { Success = false, Message = "Invalid role selected. Allowed roles are: Admin, Doctor, Receptionist." };
            }

            // Hash password using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new ApplicationUser
            {
                Username = username,
                Email = model.Email,
                FullName = model.FullName,
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
                return new AuthResult { Success = false, Message = "Email/Username and password are required." };
            }

            // Find user by username or email
            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Username.ToLower() == model.UsernameOrEmail.ToLower() || 
                u.Email.ToLower() == model.UsernameOrEmail.ToLower());

            if (user == null)
            {
                return new AuthResult { Success = false, Message = "Invalid email/username or password." };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return new AuthResult { Success = false, Message = "Invalid email/username or password." };
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

        private string GenerateJwtToken(ApplicationUser user)
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
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("Username", user.Username)
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

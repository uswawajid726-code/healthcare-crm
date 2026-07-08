using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;
using week1.Services;
using Xunit;

namespace week1.Tests
{
    public class AuthTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public AuthTests()
        {
            // Setup SQLite in-memory database connection
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Ensure database schema is created
            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Fact]
        public async Task Register_WithValidCredentials_ReturnsSuccessAndToken()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var authService = new AuthService(context);
            var model = new RegisterModel
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                Role = "Doctor",
                FullName = "Test User"
            };

            // Act
            var result = await authService.RegisterAsync(model);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Token);
            Assert.NotEmpty(result.Token);
            Assert.Equal("Registration successful.", result.Message);
            Assert.NotNull(result.User);
            Assert.Equal("testuser", result.User.Username);
            Assert.Equal("test@example.com", result.User.Email);
            Assert.Equal("Doctor", result.User.Role);
            Assert.Equal("Test User", result.User.FullName);
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ReturnsFailure()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var authService = new AuthService(context);
            
            // Seed a user
            var existingUser = new ApplicationUser
            {
                Username = "duplicateuser",
                Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Doctor",
                FullName = "Existing User"
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var model = new RegisterModel
            {
                Username = "duplicateuser",
                Email = "newemail@example.com",
                Password = "NewPassword123!",
                Role = "Doctor",
                FullName = "New User"
            };

            // Act
            var result = await authService.RegisterAsync(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Username is already taken.", result.Message);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsFailure()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var authService = new AuthService(context);

            // Seed a user
            var existingUser = new ApplicationUser
            {
                Username = "existinguser",
                Email = "duplicate@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Doctor",
                FullName = "Existing User"
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var model = new RegisterModel
            {
                Username = "newuser",
                Email = "duplicate@example.com",
                Password = "NewPassword123!",
                Role = "Doctor",
                FullName = "New User"
            };

            // Act
            var result = await authService.RegisterAsync(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Email is already registered.", result.Message);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsSuccessAndToken()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var authService = new AuthService(context);

            // Seed a user
            var user = new ApplicationUser
            {
                Username = "loginuser",
                Email = "login@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Doctor",
                FullName = "Login User"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var model = new LoginModel
            {
                UsernameOrEmail = "loginuser",
                Password = "Password123!"
            };

            // Act
            var result = await authService.LoginAsync(model);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Token);
            Assert.NotEmpty(result.Token);
            Assert.Equal("Login successful.", result.Message);
            Assert.NotNull(result.User);
            Assert.Equal("loginuser", result.User.Username);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsFailure()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var authService = new AuthService(context);

            // Seed a user
            var user = new ApplicationUser
            {
                Username = "loginuser",
                Email = "login@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Doctor",
                FullName = "Login User"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var model = new LoginModel
            {
                UsernameOrEmail = "loginuser",
                Password = "WrongPassword!"
            };

            // Act
            var result = await authService.LoginAsync(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid email/username or password.", result.Message);
            Assert.Null(result.User);
            Assert.Empty(result.Token);
        }
    }
}

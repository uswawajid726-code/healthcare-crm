using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;
using week1.Services;
using Xunit;

namespace week1.Tests
{
    public class PatientTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public PatientTests()
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
        public async Task GetPatients_ReturnsAllPatientsFromDb()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            context.Patients.Add(new Patient 
            { 
                FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", 
                PhoneNumber = "12345", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "female" 
            });
            context.Patients.Add(new Patient 
            { 
                FirstName = "Bob", LastName = "Jones", Email = "bob@example.com", 
                PhoneNumber = "67890", DateOfBirth = DateTime.Today.AddYears(-40), Gender = "male" 
            });
            await context.SaveChangesAsync();

            // Act
            var result = await patientService.GetPatientsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetPatients_WithSearchTerm_ReturnsFilteredPatients()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            context.Patients.Add(new Patient 
            { 
                FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", 
                PhoneNumber = "12345", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "female" 
            });
            context.Patients.Add(new Patient 
            { 
                FirstName = "Bob", LastName = "Jones", Email = "bob@example.com", 
                PhoneNumber = "67890", DateOfBirth = DateTime.Today.AddYears(-40), Gender = "male" 
            });
            await context.SaveChangesAsync();

            // Act
            var result = await patientService.GetPatientsAsync("Alice");

            // Assert
            Assert.Single(result);
            Assert.Equal("Alice", result.First().FirstName);
        }

        [Fact]
        public async Task AddPatient_WithValidDetails_SavesSuccessfully()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            var newPatient = new Patient
            {
                FirstName = "Charlie",
                LastName = "Brown",
                Email = "charlie@example.com",
                PhoneNumber = "5551234",
                DateOfBirth = DateTime.Today.AddYears(-15),
                Gender = "male",
                Address = "123 Main St"
            };

            // Act
            var success = await patientService.AddPatientAsync(newPatient);

            // Assert
            Assert.True(success);
            
            using var checkContext = new AppDbContext(_contextOptions);
            var savedPatient = await checkContext.Patients.FirstOrDefaultAsync(p => p.Email == "charlie@example.com");
            Assert.NotNull(savedPatient);
            Assert.Equal("Charlie", savedPatient.FirstName);
        }

        [Fact]
        public async Task AddPatient_WithDuplicateEmail_ReturnsFailure()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            context.Patients.Add(new Patient 
            { 
                FirstName = "Alice", LastName = "Smith", Email = "duplicate@example.com", 
                PhoneNumber = "12345", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "female" 
            });
            await context.SaveChangesAsync();

            var newPatient = new Patient
            {
                FirstName = "Charlie",
                LastName = "Brown",
                Email = "duplicate@example.com",
                PhoneNumber = "5551234",
                DateOfBirth = DateTime.Today.AddYears(-15),
                Gender = "male"
            };

            // Act
            var success = await patientService.AddPatientAsync(newPatient);

            // Assert
            Assert.False(success);
        }

        [Fact]
        public async Task UpdatePatient_WithValidDetails_UpdatesSuccessfully()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            var patient = new Patient 
            { 
                FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", 
                PhoneNumber = "12345", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "female",
                Address = "Old Address"
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            // Detach entity to avoid tracking issues during update
            context.Entry(patient).State = EntityState.Detached;

            var updatedPatient = new Patient
            {
                Id = patient.Id,
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                PhoneNumber = "99999", // Updated phone
                DateOfBirth = patient.DateOfBirth,
                Gender = "female",
                Address = "New Address" // Updated address
            };

            // Act
            var success = await patientService.UpdatePatientAsync(updatedPatient);

            // Assert
            Assert.True(success);

            using var checkContext = new AppDbContext(_contextOptions);
            var savedPatient = await checkContext.Patients.FindAsync(patient.Id);
            Assert.NotNull(savedPatient);
            Assert.Equal("99999", savedPatient.PhoneNumber);
            Assert.Equal("New Address", savedPatient.Address);
        }
    }
}

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
                FullName = "Alice Smith", Email = "alice@example.com", 
                Phone = "12345", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "female",
                BloodType = "O+", Status = "Active"
            });
            context.Patients.Add(new Patient 
            { 
                FullName = "Bob Jones", Email = "bob@example.com", 
                Phone = "67890", DateOfBirth = DateTime.Today.AddYears(-40), Gender = "male",
                BloodType = "A-", Status = "Active"
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
                FullName = "Alice Smith", Email = "alice@example.com", 
                Phone = "12345", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "female",
                BloodType = "B+", Status = "Active"
            });
            context.Patients.Add(new Patient 
            { 
                FullName = "Bob Jones", Email = "bob@example.com", 
                Phone = "67890", DateOfBirth = DateTime.Today.AddYears(-40), Gender = "male",
                BloodType = "O-", Status = "Active"
            });
            await context.SaveChangesAsync();

            // Act & Assert 1: Search by Name
            var resultName = await patientService.GetPatientsAsync("Alice");
            Assert.Single(resultName);
            Assert.Equal("Alice Smith", resultName.First().FullName);

            // Act & Assert 2: Search by Phone
            var resultPhone = await patientService.GetPatientsAsync("67890");
            Assert.Single(resultPhone);
            Assert.Equal("Bob Jones", resultPhone.First().FullName);
        }

        [Fact]
        public async Task AddPatient_WithValidDetails_SavesSuccessfully()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            var newPatient = new Patient
            {
                FullName = "Charlie Brown",
                Email = "charlie@example.com",
                Phone = "5551234",
                DateOfBirth = DateTime.Today.AddYears(-15),
                Gender = "male",
                Address = "123 Main St",
                BloodType = "AB+",
                Status = "Active"
            };

            // Act
            var success = await patientService.AddPatientAsync(newPatient);

            // Assert
            Assert.True(success);
            
            using var checkContext = new AppDbContext(_contextOptions);
            var savedPatient = await checkContext.Patients.FirstOrDefaultAsync(p => p.Email == "charlie@example.com");
            Assert.NotNull(savedPatient);
            Assert.Equal("Charlie Brown", savedPatient.FullName);
        }

        [Fact]
        public async Task AddPatient_WithDuplicateEmail_ReturnsFailure()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            context.Patients.Add(new Patient 
            { 
                FullName = "Alice Smith", Email = "duplicate@example.com", 
                Phone = "12345", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "female",
                BloodType = "A+", Status = "Active"
            });
            await context.SaveChangesAsync();

            var newPatient = new Patient
            {
                FullName = "Charlie Brown",
                Email = "duplicate@example.com",
                Phone = "5551234",
                DateOfBirth = DateTime.Today.AddYears(-15),
                Gender = "male",
                BloodType = "O-",
                Status = "Active"
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
                FullName = "Alice Smith", Email = "alice@example.com", 
                Phone = "12345", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "female",
                Address = "Old Address", BloodType = "A+", Status = "Active"
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            // Detach entity to avoid tracking issues during update
            context.Entry(patient).State = EntityState.Detached;

            var updatedPatient = new Patient
            {
                Id = patient.Id,
                FullName = "Alice Smith",
                Email = "alice@example.com",
                Phone = "99999", // Updated phone
                DateOfBirth = patient.DateOfBirth,
                Gender = "female",
                Address = "New Address", // Updated address
                BloodType = "A+",
                Status = "Active"
            };

            // Act
            var success = await patientService.UpdatePatientAsync(updatedPatient);

            // Assert
            Assert.True(success);

            using var checkContext = new AppDbContext(_contextOptions);
            var savedPatient = await checkContext.Patients.FindAsync(patient.Id);
            Assert.NotNull(savedPatient);
            Assert.Equal("99999", savedPatient.Phone);
            Assert.Equal("New Address", savedPatient.Address);
        }

        [Fact]
        public async Task UpdatePatient_NonExistent_ReturnsFalse()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            var nonExistentPatient = new Patient
            {
                Id = 9999,
                FullName = "Non Existent",
                Email = "noone@example.com",
                Phone = "00000",
                DateOfBirth = DateTime.Today,
                Gender = "other",
                BloodType = "O+",
                Status = "Active"
            };

            // Act
            var result = await patientService.UpdatePatientAsync(nonExistentPatient);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddMedicalHistory_Valid_SavesSuccessfully()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            var patient = new Patient 
            { 
                FullName = "Patient X", Email = "x@example.com", 
                Phone = "111", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "male",
                BloodType = "B-", Status = "Active"
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var history = new MedicalHistory
            {
                PatientId = patient.Id,
                Diagnosis = "Flu",
                Notes = "Prescribed Ibuprofen and rest."
            };

            // Act
            var success = await patientService.AddMedicalHistoryAsync(history);

            // Assert
            Assert.True(success);
            Assert.True(history.Id > 0);

            using var checkContext = new AppDbContext(_contextOptions);
            var savedHistory = await checkContext.MedicalHistories.FirstOrDefaultAsync(h => h.Id == history.Id);
            Assert.NotNull(savedHistory);
            Assert.Equal("Flu", savedHistory.Diagnosis);
            Assert.Equal("Prescribed Ibuprofen and rest.", savedHistory.Notes);
            Assert.Equal(patient.Id, savedHistory.PatientId);
        }

        [Fact]
        public async Task GetMedicalHistory_ByPatientId_ReturnsHistoryList()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            var patientService = new PatientService(context);

            var patient = new Patient 
            { 
                FullName = "Patient Y", Email = "y@example.com", 
                Phone = "222", DateOfBirth = DateTime.Today.AddYears(-20), Gender = "female",
                BloodType = "A+", Status = "Active"
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            context.MedicalHistories.Add(new MedicalHistory
            {
                PatientId = patient.Id,
                Diagnosis = "Diagnosis 1",
                Notes = "Notes 1"
            });
            context.MedicalHistories.Add(new MedicalHistory
            {
                PatientId = patient.Id,
                Diagnosis = "Diagnosis 2",
                Notes = "Notes 2"
            });
            await context.SaveChangesAsync();

            // Act
            var historyList = await patientService.GetMedicalHistoryByPatientIdAsync(patient.Id);

            // Assert
            Assert.Equal(2, historyList.Count());
            Assert.Contains(historyList, h => h.Diagnosis == "Diagnosis 1");
            Assert.Contains(historyList, h => h.Diagnosis == "Diagnosis 2");
        }
    }
}

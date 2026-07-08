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
    public class AppointmentTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public AppointmentTests()
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

        private async Task SeedDataAsync(AppDbContext context)
        {
            // Seed Doctors
            var doc1 = new ApplicationUser { Id = 10, FullName = "Dr. Sarah Connor", Email = "doctor@example.com", Username = "doctor@example.com", PasswordHash = "hash", Role = "Doctor" };
            var doc2 = new ApplicationUser { Id = 11, FullName = "Dr. John Watson", Email = "doctor2@example.com", Username = "doctor2@example.com", PasswordHash = "hash", Role = "Doctor" };
            context.Users.AddRange(doc1, doc2);

            // Seed Patients
            var patient1 = new Patient { Id = 20, FullName = "Jane Doe", DateOfBirth = DateTime.Today.AddYears(-30), Gender = "Female", Phone = "12345", Email = "jane@example.com", BloodType = "O+", Status = "Active" };
            var patient2 = new Patient { Id = 21, FullName = "John Smith", DateOfBirth = DateTime.Today.AddYears(-40), Gender = "Male", Phone = "67890", Email = "john@example.com", BloodType = "A-", Status = "Active" };
            context.Patients.AddRange(patient1, patient2);

            await context.SaveChangesAsync();

            // Seed Appointments
            context.Appointments.Add(new Appointment
            {
                Id = 1,
                PatientId = 20,
                DoctorId = 10,
                AppointmentDate = DateTime.Today.AddDays(1),
                AppointmentTime = "10:00 AM",
                Reason = "Checkup 1",
                Status = "Scheduled"
            });
            context.Appointments.Add(new Appointment
            {
                Id = 2,
                PatientId = 21,
                DoctorId = 10,
                AppointmentDate = DateTime.Today.AddDays(2),
                AppointmentTime = "11:30 AM",
                Reason = "Diabetes Consultation",
                Status = "Scheduled"
            });
            context.Appointments.Add(new Appointment
            {
                Id = 3,
                PatientId = 20,
                DoctorId = 11,
                AppointmentDate = DateTime.Today,
                AppointmentTime = "02:00 PM",
                Reason = "Follow-up consultation",
                Status = "Completed"
            });

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAppointments_ReturnsAllAppointments()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            // Act
            var result = await appointmentService.GetAppointmentsAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAppointments_ForSpecificDoctor_FiltersCorrectly()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            // Act (doctor 10 has 2 appointments, doctor 11 has 1)
            var resultDoc10 = await appointmentService.GetAppointmentsAsync(doctorId: 10);
            var resultDoc11 = await appointmentService.GetAppointmentsAsync(doctorId: 11);

            // Assert
            Assert.Equal(2, resultDoc10.Count());
            Assert.Single(resultDoc11);
        }

        [Fact]
        public async Task GetAppointments_WithSearchTerm_FiltersCorrectly()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            // Act: Search for patient "Smith"
            var resultSmith = await appointmentService.GetAppointmentsAsync(search: "Smith");

            // Act: Search for doctor "Watson"
            var resultWatson = await appointmentService.GetAppointmentsAsync(search: "Watson");

            // Assert
            Assert.Single(resultSmith);
            Assert.Equal("John Smith", resultSmith.First().Patient!.FullName);

            Assert.Single(resultWatson);
            Assert.Equal("Dr. John Watson", resultWatson.First().Doctor!.FullName);
        }

        [Fact]
        public async Task GetAppointments_WithStatusFilter_FiltersCorrectly()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            // Act
            var scheduled = await appointmentService.GetAppointmentsAsync(status: "Scheduled");
            var completed = await appointmentService.GetAppointmentsAsync(status: "Completed");

            // Assert
            Assert.Equal(2, scheduled.Count());
            Assert.Single(completed);
        }

        [Fact]
        public async Task AddAppointment_WithValidDetails_SavesSuccessfully()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            var newAppt = new Appointment
            {
                PatientId = 20,
                DoctorId = 10,
                AppointmentDate = DateTime.Today.AddDays(5),
                AppointmentTime = "04:00 PM",
                Reason = "Valid New Consultation",
                Status = "Scheduled"
            };

            // Act
            var success = await appointmentService.AddAppointmentAsync(newAppt);

            // Assert
            Assert.True(success);
            Assert.True(newAppt.Id > 3);

            using var checkContext = new AppDbContext(_contextOptions);
            var savedAppt = await checkContext.Appointments.FindAsync(newAppt.Id);
            Assert.NotNull(savedAppt);
            Assert.Equal("Valid New Consultation", savedAppt.Reason);
        }

        [Fact]
        public async Task AddAppointment_WithNonExistentPatientOrDoctor_ReturnsFailure()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            // Patient 999 does not exist
            var invalidAppt1 = new Appointment
            {
                PatientId = 999,
                DoctorId = 10,
                AppointmentDate = DateTime.Today,
                AppointmentTime = "10:00 AM",
                Reason = "Invalid Patient",
                Status = "Scheduled"
            };

            // Doctor 999 does not exist
            var invalidAppt2 = new Appointment
            {
                PatientId = 20,
                DoctorId = 999,
                AppointmentDate = DateTime.Today,
                AppointmentTime = "10:00 AM",
                Reason = "Invalid Doctor",
                Status = "Scheduled"
            };

            // Act
            var result1 = await appointmentService.AddAppointmentAsync(invalidAppt1);
            var result2 = await appointmentService.AddAppointmentAsync(invalidAppt2);

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        public async Task UpdateAppointment_ValidDetails_UpdatesSuccessfully()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            var original = await context.Appointments.FindAsync(1);
            Assert.NotNull(original);
            context.Entry(original).State = EntityState.Detached;

            var updated = new Appointment
            {
                Id = 1,
                PatientId = original.PatientId,
                DoctorId = original.DoctorId,
                AppointmentDate = original.AppointmentDate,
                AppointmentTime = "09:30 AM", // Updated time
                Reason = "Updated Reason",
                Status = "Completed" // Updated status
            };

            // Act
            var success = await appointmentService.UpdateAppointmentAsync(updated);

            // Assert
            Assert.True(success);

            using var checkContext = new AppDbContext(_contextOptions);
            var saved = await checkContext.Appointments.FindAsync(1);
            Assert.NotNull(saved);
            Assert.Equal("09:30 AM", saved.AppointmentTime);
            Assert.Equal("Updated Reason", saved.Reason);
            Assert.Equal("Completed", saved.Status);
        }

        [Fact]
        public async Task DeleteAppointment_ValidId_DeletesSuccessfully()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            // Act
            var success = await appointmentService.DeleteAppointmentAsync(1);

            // Assert
            Assert.True(success);

            using var checkContext = new AppDbContext(_contextOptions);
            var deleted = await checkContext.Appointments.FindAsync(1);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteAppointment_NonExistentId_ReturnsFalse()
        {
            // Arrange
            using var context = new AppDbContext(_contextOptions);
            await SeedDataAsync(context);
            var appointmentService = new AppointmentService(context);

            // Act
            var success = await appointmentService.DeleteAppointmentAsync(999);

            // Assert
            Assert.False(success);
        }
    }
}

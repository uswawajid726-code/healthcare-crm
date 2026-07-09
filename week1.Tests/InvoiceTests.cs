using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;
using week1.Services;
using Xunit;

// NOTE: These tests assume Member C's backend contract:
//   Invoice { Id, AppointmentId, PatientId, Amount, Status ("Unpaid"/"Paid"/"Overdue"), IssuedAt, PaidAt }
//   IInvoiceService.CreateInvoiceAsync(Invoice), MarkAsPaidAsync(int id), GetInvoicesAsync(string? status)
// Adjust member/method names below if backend implementation differs slightly.

namespace week1.Tests
{
    public class InvoiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public InvoiceTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        private async Task<(Patient patient, Appointment appt)> SeedBaseDataAsync(AppDbContext context)
        {
            var doctor = new ApplicationUser
            {
                Id = 10,
                FullName = "Dr. Sarah Connor",
                Email = "doctor@example.com",
                Username = "doctor@example.com",
                PasswordHash = "hash",
                Role = "Doctor"
            };
            context.Users.Add(doctor);

            var patient = new Patient
            {
                Id = 20,
                FullName = "Jane Doe",
                DateOfBirth = DateTime.Today.AddYears(-30),
                Gender = "Female",
                Phone = "12345",
                Email = "jane@example.com",
                BloodType = "O+",
                Status = "Active"
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var appt = new Appointment
            {
                Id = 1,
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentDate = DateTime.Today,
                AppointmentTime = "10:00 AM",
                Reason = "Routine Checkup",
                Status = "Completed"
            };
            context.Appointments.Add(appt);
            await context.SaveChangesAsync();

            return (patient, appt);
        }

        // Test Case 1: Invoice generation linked to an appointment
        [Fact]
        public async Task GenerateInvoice_LinkedToAppointment_CreatesUnpaidInvoice()
        {
            using var context = new AppDbContext(_contextOptions);
            var (patient, appt) = await SeedBaseDataAsync(context);
            var service = new InvoiceService(context);

            var invoice = new Invoice
            {
                AppointmentId = appt.Id,
                PatientId = patient.Id,
                Amount = 2500,
                Status = "Unpaid"
            };

            var success = await service.CreateInvoiceAsync(invoice);

            Assert.True(success);
            Assert.NotEqual(0, invoice.Id);
            Assert.Equal("Unpaid", invoice.Status);
            Assert.Equal(appt.Id, invoice.AppointmentId);
            Assert.Null(invoice.PaidAt);
        }

        // Test Case 2: Mark invoice as Paid and record payment timestamp
        [Fact]
        public async Task MarkAsPaid_ValidUnpaidInvoice_UpdatesStatusAndTimestamp()
        {
            using var context = new AppDbContext(_contextOptions);
            var (patient, appt) = await SeedBaseDataAsync(context);
            var service = new InvoiceService(context);

            var invoice = new Invoice { AppointmentId = appt.Id, PatientId = patient.Id, Amount = 1500, Status = "Unpaid" };
            await service.CreateInvoiceAsync(invoice);

            var beforePay = DateTime.UtcNow;
            var result = await service.MarkAsPaidAsync(invoice.Id);

            Assert.True(result);

            var updated = await service.GetInvoiceByIdAsync(invoice.Id);
            Assert.NotNull(updated);
            Assert.Equal("Paid", updated!.Status);
            Assert.NotNull(updated.PaidAt);
            Assert.True(updated.PaidAt >= beforePay);
        }

        // Test Case 3: Marking an already-paid invoice again should not succeed / no duplicate payment
        [Fact]
        public async Task MarkAsPaid_AlreadyPaidInvoice_ReturnsFalse()
        {
            using var context = new AppDbContext(_contextOptions);
            var (patient, appt) = await SeedBaseDataAsync(context);
            var service = new InvoiceService(context);

            var invoice = new Invoice { AppointmentId = appt.Id, PatientId = patient.Id, Amount = 1000, Status = "Unpaid" };
            await service.CreateInvoiceAsync(invoice);
            await service.MarkAsPaidAsync(invoice.Id);

            var secondAttempt = await service.MarkAsPaidAsync(invoice.Id);

            Assert.False(secondAttempt);
        }

        // Test Case 4: Filtering invoices by status (Paid / Unpaid / Overdue)
        [Fact]
        public async Task GetInvoices_FilterByStatus_ReturnsOnlyMatchingInvoices()
        {
            using var context = new AppDbContext(_contextOptions);
            var (patient, appt) = await SeedBaseDataAsync(context);
            var service = new InvoiceService(context);

            var unpaid = new Invoice { AppointmentId = appt.Id, PatientId = patient.Id, Amount = 1000, Status = "Unpaid" };
            var paid = new Invoice { AppointmentId = appt.Id, PatientId = patient.Id, Amount = 2000, Status = "Unpaid" };
            var overdue = new Invoice { AppointmentId = appt.Id, PatientId = patient.Id, Amount = 3000, Status = "Overdue" };

            await service.CreateInvoiceAsync(unpaid);
            await service.CreateInvoiceAsync(paid);
            await service.MarkAsPaidAsync(paid.Id);
            await service.CreateInvoiceAsync(overdue);

            var paidResults = await service.GetInvoicesAsync("Paid");
            var unpaidResults = await service.GetInvoicesAsync("Unpaid");
            var overdueResults = await service.GetInvoicesAsync("Overdue");

            Assert.Single(paidResults);
            Assert.Equal(paid.Id, paidResults.First().Id);

            Assert.Single(unpaidResults);
            Assert.Equal(unpaid.Id, unpaidResults.First().Id);

            Assert.Single(overdueResults);
            Assert.Equal(overdue.Id, overdueResults.First().Id);
        }

        // Test Case 5: Invoice amount and patient linkage integrity
        [Fact]
        public async Task GetInvoiceById_ReturnsCorrectPatientAndAmount()
        {
            using var context = new AppDbContext(_contextOptions);
            var (patient, appt) = await SeedBaseDataAsync(context);
            var service = new InvoiceService(context);

            var invoice = new Invoice { AppointmentId = appt.Id, PatientId = patient.Id, Amount = 4200, Status = "Unpaid" };
            await service.CreateInvoiceAsync(invoice);

            var fetched = await service.GetInvoiceByIdAsync(invoice.Id);

            Assert.NotNull(fetched);
            Assert.Equal(4200, fetched!.Amount);
            Assert.Equal(patient.Id, fetched.PatientId);
        }
    }
}
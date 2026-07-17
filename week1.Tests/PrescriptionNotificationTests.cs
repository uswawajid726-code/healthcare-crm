using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using week1.Controllers;
using week1.Data;
using week1.Models;
using Xunit;

namespace week1.Tests
{
	public class PrescriptionNotificationTests : IDisposable
	{
		private readonly SqliteConnection _connection;
		private readonly DbContextOptions<AppDbContext> _contextOptions;

		public PrescriptionNotificationTests()
		{
			_connection = new SqliteConnection("Filename=:memory:");
			_connection.Open();

			_contextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseSqlite(_connection)
				.Options;

			using var context = new AppDbContext(_contextOptions);
			context.Database.EnsureCreated();
		}

		public void Dispose() => _connection.Dispose();

		[Fact]
		public async Task CreatePrescription_ValidData_LinksToAppointment()
		{
			using var context = new AppDbContext(_contextOptions);
			var controller = new PrescriptionsController(context);

			var prescription = new Prescription
			{
				AppointmentId = 101,
				Medicine = "Amoxicillin",
				Dosage = "500mg twice daily",
				Instructions = "After meals"
			};

			var result = await controller.CreatePrescription(prescription);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var saved = Assert.IsType<Prescription>(okResult.Value);
			Assert.Equal(101, saved.AppointmentId);
			Assert.NotEqual(0, saved.Id);
		}

		[Fact]
		public async Task GetPrescriptions_FilterByAppointmentId_ReturnsOnlyMatching()
		{
			using var context = new AppDbContext(_contextOptions);
			var controller = new PrescriptionsController(context);

			await controller.CreatePrescription(new Prescription { AppointmentId = 1, Medicine = "Paracetamol", Dosage = "500mg" });
			await controller.CreatePrescription(new Prescription { AppointmentId = 2, Medicine = "Ibuprofen", Dosage = "200mg" });

			var result = await controller.GetPrescriptions(1);
			var okResult = Assert.IsType<OkObjectResult>(result);
			var list = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Prescription>>(okResult.Value).ToList();

			Assert.Single(list);
			Assert.Equal("Paracetamol", list.First().Medicine);
		}

		[Fact]
		public async Task CreateNotification_ValidData_CreatesUnreadNotification()
		{
			using var context = new AppDbContext(_contextOptions);
			var controller = new NotificationsController(context);

			var notification = new Notification
			{
				UserId = 5,
				Title = "Appointment Reminder",
				Message = "Your appointment is in 24 hours.",
				RelatedUrl = "/AppointmentView/Details/101"
			};

			var result = await controller.CreateNotification(notification);
			var okResult = Assert.IsType<OkObjectResult>(result);
			var saved = Assert.IsType<Notification>(okResult.Value);

			Assert.False(saved.IsRead);
			Assert.Equal(5, saved.UserId);
		}

		[Fact]
		public async Task GetNotifications_ReturnsOnlyUnreadForUser()
		{
			using var context = new AppDbContext(_contextOptions);
			var controller = new NotificationsController(context);

			await controller.CreateNotification(new Notification { UserId = 7, Title = "A", Message = "msg1" });
			var created = await controller.CreateNotification(new Notification { UserId = 7, Title = "B", Message = "msg2" });
			var savedNotif = (Notification)((OkObjectResult)created).Value!;
			await controller.MarkAsRead(savedNotif.Id);

			var result = await controller.GetNotifications(7);
			var okResult = Assert.IsType<OkObjectResult>(result);
			var list = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Notification>>(okResult.Value).ToList();

			Assert.Single(list);
			Assert.Equal("A", list.First().Title);
		}

		[Fact]
		public async Task MarkAsRead_ValidNotification_UpdatesIsReadTrue()
		{
			using var context = new AppDbContext(_contextOptions);
			var controller = new NotificationsController(context);

			var created = await controller.CreateNotification(new Notification { UserId = 9, Title = "Test", Message = "msg" });
			var savedNotif = (Notification)((OkObjectResult)created).Value!;

			var result = await controller.MarkAsRead(savedNotif.Id);
			var okResult = Assert.IsType<OkObjectResult>(result);
			var updated = Assert.IsType<Notification>(okResult.Value);

			Assert.True(updated.IsRead);
		}
	}
}
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
    public class EmergencyContactTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public EmergencyContactTests()
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
        public async Task AddContact_ValidData_CreatesContact()
        {
            using var context = new AppDbContext(_contextOptions);
            var controller = new EmergencyContactsController(context);

            var contact = new EmergencyContact { PatientId = 20, Name = "John Doe", Phone = "03001234567", Relationship = "Spouse" };
            var result = await controller.AddContact(contact);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var saved = Assert.IsType<EmergencyContact>(okResult.Value);
            Assert.Equal(20, saved.PatientId);
        }

        [Fact]
        public async Task GetContacts_FilterByPatientId_ReturnsOnlyMatching()
        {
            using var context = new AppDbContext(_contextOptions);
            var controller = new EmergencyContactsController(context);

            await controller.AddContact(new EmergencyContact { PatientId = 1, Name = "A", Phone = "111" });
            await controller.AddContact(new EmergencyContact { PatientId = 2, Name = "B", Phone = "222" });

            var result = await controller.GetContacts(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<EmergencyContact>>(okResult.Value).ToList();

            Assert.Single(list);
            Assert.Equal("A", list.First().Name);
        }

        [Fact]
        public async Task DeleteContact_ExistingContact_RemovesSuccessfully()
        {
            using var context = new AppDbContext(_contextOptions);
            var controller = new EmergencyContactsController(context);

            var added = await controller.AddContact(new EmergencyContact { PatientId = 3, Name = "Test", Phone = "333" });
            var saved = (EmergencyContact)((OkObjectResult)added).Value!;

            var deleteResult = await controller.DeleteContact(saved.Id);
            Assert.IsType<OkObjectResult>(deleteResult);

            var getResult = await controller.GetContacts(3);
            var list = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<EmergencyContact>>(((OkObjectResult)getResult).Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task DeleteContact_NonExistentId_ReturnsNotFound()
        {
            using var context = new AppDbContext(_contextOptions);
            var controller = new EmergencyContactsController(context);

            var result = await controller.DeleteContact(9999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task NotifyContact_ValidContact_ReturnsSuccessMessage()
        {
            using var context = new AppDbContext(_contextOptions);
            var controller = new EmergencyContactsController(context);

            var added = await controller.AddContact(new EmergencyContact { PatientId = 4, Name = "Jane", Phone = "444" });
            var saved = (EmergencyContact)((OkObjectResult)added).Value!;

            var result = await controller.NotifyContact(saved.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
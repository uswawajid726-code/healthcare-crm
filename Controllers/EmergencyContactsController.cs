using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyContactsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmergencyContactsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/emergencycontacts
        [HttpGet]
        public async Task<IActionResult> GetEmergencyContacts()
        {
            return Ok(await _context.EmergencyContacts.ToListAsync());
        }

        // POST: api/emergencycontacts
        [HttpPost]
        public async Task<IActionResult> CreateEmergencyContact(EmergencyContact contact)
        {
            contact.CreatedAt = DateTime.UtcNow;

            _context.EmergencyContacts.Add(contact);
            await _context.SaveChangesAsync();

            return Ok(contact);
        }

        // DELETE: api/emergencycontacts/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmergencyContact(int id)
        {
            var contact = await _context.EmergencyContacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            _context.EmergencyContacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Emergency contact deleted successfully." });
        }
    }
}
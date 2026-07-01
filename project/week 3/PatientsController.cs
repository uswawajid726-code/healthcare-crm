using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Models;
using HospitalApp.Data;

namespace HospitalApp.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize] // requires valid JWT — built in Week 2
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/patients
        // GET /api/patients?search=ahmed
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients([FromQuery] string? search)
        {
            var query = _context.Patients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(p =>
                    p.FullName.Contains(search) ||
                    p.Phone.Contains(search));
            }

            var patients = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(patients);
        }

        // GET /api/patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.MedicalHistories)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            return Ok(patient);
        }

        // POST /api/patients
        [HttpPost]
        public async Task<ActionResult<Patient>> CreatePatient([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Prevent duplicate phone numbers
            bool phoneExists = await _context.Patients
                .AnyAsync(p => p.Phone == patient.Phone);

            if (phoneExists)
                return BadRequest(new { message = "A patient with this phone number already exists." });

            patient.CreatedAt = DateTime.UtcNow;
            patient.Status = string.IsNullOrWhiteSpace(patient.Status) ? "Active" : patient.Status;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
        }

        // PUT /api/patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient updated)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            patient.FullName = updated.FullName;
            patient.DateOfBirth = updated.DateOfBirth;
            patient.Gender = updated.Gender;
            patient.Phone = updated.Phone;
            patient.Email = updated.Email;
            patient.Address = updated.Address;
            patient.BloodType = updated.BloodType;
            patient.Status = updated.Status;
            patient.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(patient);
        }

        // DELETE /api/patients/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // only Admin can delete — role-based access from Week 2
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

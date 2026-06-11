using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetPatientsAsync(string? search = null)
        {
            IQueryable<Patient> query = _context.Patients;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower().Trim();
                query = query.Where(p => 
                    p.FirstName.ToLower().Contains(lowerSearch) || 
                    p.LastName.ToLower().Contains(lowerSearch) || 
                    p.Email.ToLower().Contains(lowerSearch) ||
                    p.PhoneNumber.Contains(lowerSearch));
            }

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task<bool> AddPatientAsync(Patient patient)
        {
            if (patient == null) return false;

            // Validate duplicate email
            if (await _context.Patients.AnyAsync(p => p.Email.ToLower() == patient.Email.ToLower()))
            {
                return false;
            }

            patient.CreatedAt = DateTime.UtcNow;
            _context.Patients.Add(patient);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            if (patient == null) return false;

            // Check if patient exists
            var existing = await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patient.Id);
            if (existing == null) return false;

            // Check duplicate email for other patients
            if (await _context.Patients.AnyAsync(p => p.Email.ToLower() == patient.Email.ToLower() && p.Id != patient.Id))
            {
                return false;
            }

            patient.CreatedAt = existing.CreatedAt; // Preserve original creation timestamp
            _context.Patients.Update(patient);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;

            _context.Patients.Remove(patient);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}

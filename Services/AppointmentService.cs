using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync(string? search = null, string? status = null, DateTime? date = null, int? doctorId = null)
        {
            IQueryable<Appointment> query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor);

            // Filter by Doctor if restricted
            if (doctorId.HasValue)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }

            // Search by Patient or Doctor name
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower().Trim();
                query = query.Where(a => 
                    (a.Patient != null && a.Patient.FullName.ToLower().Contains(searchLower)) ||
                    (a.Doctor != null && a.Doctor.FullName.ToLower().Contains(searchLower)));
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status.ToLower() == status.ToLower().Trim());
            }

            // Filter by date
            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(a => a.AppointmentDate.Date == targetDate);
            }

            return await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            if (appointment == null) return false;

            // Verify Patient and Doctor exist
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
            var doctorExists = await _context.Users.AnyAsync(u => u.Id == appointment.DoctorId && u.Role == "Doctor");

            if (!patientExists || !doctorExists)
            {
                return false;
            }

            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Add(appointment);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            if (appointment == null) return false;

            var existing = await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == appointment.Id);
            if (existing == null) return false;

            var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
            var doctorExists = await _context.Users.AnyAsync(u => u.Id == appointment.DoctorId && u.Role == "Doctor");

            if (!patientExists || !doctorExists)
            {
                return false;
            }

            appointment.CreatedAt = existing.CreatedAt;
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Update(appointment);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            _context.Appointments.Remove(appointment);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<ApplicationUser>> GetDoctorsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Doctor")
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }
    }
}

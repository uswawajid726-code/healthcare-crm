using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _context;

        public AnalyticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PatientAnalyticsDto> GetPatientAnalyticsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var totalPatients = await _context.Patients.CountAsync();

            var newPatientsThisMonth = await _context.Patients
                .CountAsync(p => p.CreatedAt >= startOfMonth);

            var genderGrouped = await _context.Patients
                .GroupBy(p => string.IsNullOrEmpty(p.Gender) ? "Unspecified" : p.Gender)
                .Select(g => new { Gender = g.Key, Count = g.Count() })
                .ToListAsync();

            var genderDict = genderGrouped.ToDictionary(g => g.Gender, g => g.Count);

            return new PatientAnalyticsDto
            {
                TotalPatients = totalPatients,
                NewPatientsThisMonth = newPatientsThisMonth,
                GenderDistribution = genderDict
            };
        }

        public async Task<AppointmentAnalyticsDto> GetAppointmentAnalyticsAsync()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var startDate = today.AddDays(-29); // Last 30 days including today

            var totalAppointments = await _context.Appointments.CountAsync();
            var todayAppointments = await _context.Appointments
                .CountAsync(a => a.AppointmentDate.Date == today);

            // Fetch appointments in range
            var appointmentsInRange = await _context.Appointments
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= today.AddDays(1).AddTicks(-1))
                .ToListAsync();

            var appointmentCountsByDate = appointmentsInRange
                .GroupBy(a => a.AppointmentDate.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.Count());

            var dailyList = new List<DailyAppointmentCountDto>();
            for (int i = 0; i < 30; i++)
            {
                var currentDate = startDate.AddDays(i);
                var dateStr = currentDate.ToString("yyyy-MM-dd");
                dailyList.Add(new DailyAppointmentCountDto
                {
                    Date = dateStr,
                    Count = appointmentCountsByDate.TryGetValue(dateStr, out int count) ? count : 0
                });
            }

            return new AppointmentAnalyticsDto
            {
                DailyAppointments = dailyList,
                TotalAppointments = totalAppointments,
                TodayAppointments = todayAppointments
            };
        }

        public async Task<DoctorAnalyticsDto> GetDoctorAnalyticsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var doctors = await _context.Users
                .Where(u => u.Role == "Doctor")
                .ToListAsync();

            var doctorIds = doctors.Select(d => d.Id).ToList();

            var monthlyAppointments = await _context.Appointments
                .Where(a => doctorIds.Contains(a.DoctorId) && a.AppointmentDate >= startOfMonth)
                .GroupBy(a => a.DoctorId)
                .Select(g => new { DoctorId = g.Key, Count = g.Count() })
                .ToListAsync();

            var countDict = monthlyAppointments.ToDictionary(m => m.DoctorId, m => m.Count);

            var doctorList = doctors.Select(d => new DoctorAppointmentCountDto
            {
                DoctorId = d.Id,
                DoctorName = string.IsNullOrWhiteSpace(d.FullName) ? d.Username : d.FullName,
                AppointmentCount = countDict.TryGetValue(d.Id, out int count) ? count : 0
            })
            .OrderByDescending(d => d.AppointmentCount)
            .ToList();

            return new DoctorAnalyticsDto
            {
                DoctorAppointments = doctorList,
                TotalDoctors = doctors.Count
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using week1.Models;

namespace week1.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAppointmentsAsync(string? search = null, string? status = null, DateTime? date = null, int? doctorId = null);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<bool> AddAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<IEnumerable<ApplicationUser>> GetDoctorsAsync();
    }
}

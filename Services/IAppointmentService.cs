using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using week1.Models;

namespace week1.Services
{
    /// <summary>
    /// Service contract for managing appointment schedules, consultations, and doctor assignments.
    /// </summary>
    public interface IAppointmentService
    {
        /// <summary>
        /// Retrieves appointments based on optional filters (search string, status, date, doctor ID).
        /// </summary>
        Task<IEnumerable<Appointment>> GetAppointmentsAsync(string? search = null, string? status = null, DateTime? date = null, int? doctorId = null);

        /// <summary>
        /// Retrieves a specific appointment by ID.
        /// </summary>
        Task<Appointment?> GetAppointmentByIdAsync(int id);

        /// <summary>
        /// Schedules a new appointment.
        /// </summary>
        Task<bool> AddAppointmentAsync(Appointment appointment);

        /// <summary>
        /// Updates an existing appointment.
        /// </summary>
        Task<bool> UpdateAppointmentAsync(Appointment appointment);

        /// <summary>
        /// Cancels or deletes an appointment by ID.
        /// </summary>
        Task<bool> DeleteAppointmentAsync(int id);

        /// <summary>
        /// Retrieves a list of active doctors for appointment scheduling.
        /// </summary>
        Task<IEnumerable<ApplicationUser>> GetDoctorsAsync();

        /// <summary>
        /// Checks if a doctor is available at the specified date and time (prevents double booking).
        /// </summary>
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime date, string time, int? excludeAppointmentId = null);
    }
}

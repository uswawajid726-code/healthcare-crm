using System.Threading.Tasks;
using week1.Models;

namespace week1.Services
{
    public interface IAnalyticsService
    {
        Task<PatientAnalyticsDto> GetPatientAnalyticsAsync();
        Task<AppointmentAnalyticsDto> GetAppointmentAnalyticsAsync();
        Task<DoctorAnalyticsDto> GetDoctorAnalyticsAsync();
    }
}

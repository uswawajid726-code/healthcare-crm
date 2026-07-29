using System.Collections.Generic;

namespace week1.Models
{
    public class PatientAnalyticsDto
    {
        public int TotalPatients { get; set; }
        public int NewPatientsThisMonth { get; set; }
        public Dictionary<string, int> GenderDistribution { get; set; } = new Dictionary<string, int>();
    }

    public class DailyAppointmentCountDto
    {
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AppointmentAnalyticsDto
    {
        public List<DailyAppointmentCountDto> DailyAppointments { get; set; } = new List<DailyAppointmentCountDto>();
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
    }

    public class DoctorAppointmentCountDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
    }

    public class DoctorAnalyticsDto
    {
        public List<DoctorAppointmentCountDto> DoctorAppointments { get; set; } = new List<DoctorAppointmentCountDto>();
        public int TotalDoctors { get; set; }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using week1.Models;

namespace week1.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetPatientsAsync(string? search = null);
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<bool> AddPatientAsync(Patient patient);
        Task<bool> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int id);
    }
}

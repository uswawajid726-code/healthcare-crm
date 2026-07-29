using System.Collections.Generic;
using System.Threading.Tasks;
using week1.Models;

namespace week1.Services
{
    /// <summary>
    /// Service contract for patient records and medical history management.
    /// </summary>
    public interface IPatientService
    {
        /// <summary>
        /// Retrieves a collection of patients, optionally filtered by a search string.
        /// </summary>
        Task<IEnumerable<Patient>> GetPatientsAsync(string? search = null);

        /// <summary>
        /// Retrieves a specific patient by ID.
        /// </summary>
        Task<Patient?> GetPatientByIdAsync(int id);

        /// <summary>
        /// Adds a new patient record to the system.
        /// </summary>
        Task<bool> AddPatientAsync(Patient patient);

        /// <summary>
        /// Updates an existing patient record.
        /// </summary>
        Task<bool> UpdatePatientAsync(Patient patient);

        /// <summary>
        /// Deletes a patient record by ID.
        /// </summary>
        Task<bool> DeletePatientAsync(int id);

        /// <summary>
        /// Retrieves medical history entries associated with a patient.
        /// </summary>
        Task<IEnumerable<MedicalHistory>> GetMedicalHistoryByPatientIdAsync(int patientId);

        /// <summary>
        /// Adds a new medical history record for a patient.
        /// </summary>
        Task<bool> AddMedicalHistoryAsync(MedicalHistory history);
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using week1.Models;
using week1.Services;

namespace week1.Controllers
{
    /// <summary>
    /// API controller for managing patient records and medical histories.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Retrieves a list of all patients, optionally filtered by a search query (name or phone).
        /// </summary>
        /// <param name="search">Optional query to search by name or contact number.</param>
        /// <returns>A list of patients.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> GetPatients([FromQuery] string? search = null)
        {
            try
            {
                var patients = await _patientService.GetPatientsAsync(search);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Patients retrieved successfully.",
                    Data = patients
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving patients: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Retrieves a single patient's details by their ID.
        /// </summary>
        /// <param name="id">The unique ID of the patient.</param>
        /// <returns>The patient details.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetPatient(int id)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Patient with ID {id} not found."
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Patient details retrieved successfully.",
                    Data = patient
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving patient details: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Registers a new patient in the CRM database.
        /// </summary>
        /// <param name="patient">The patient details to add.</param>
        /// <returns>The created patient object.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> AddPatient([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid patient data model."
                });
            }

            try
            {
                var success = await _patientService.AddPatientAsync(patient);
                if (!success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Failed to create patient. Email may already be registered."
                    });
                }

                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, new ApiResponse
                {
                    Success = true,
                    Message = "Patient created successfully.",
                    Data = patient
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while adding the patient: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Updates an existing patient's details in the CRM.
        /// </summary>
        /// <param name="id">The ID of the patient to update.</param>
        /// <param name="patient">The updated patient details.</param>
        /// <returns>The updated patient object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Patient ID mismatch."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid patient data model."
                });
            }

            try
            {
                var success = await _patientService.UpdatePatientAsync(patient);
                if (!success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Failed to update patient. Patient may not exist or email is already registered to another patient."
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Patient details updated successfully.",
                    Data = patient
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating the patient: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Deletes a patient record by their ID.
        /// </summary>
        /// <param name="id">The unique ID of the patient to delete.</param>
        /// <returns>A status response.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                var success = await _patientService.DeletePatientAsync(id);
                if (!success)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Patient with ID {id} not found or could not be deleted."
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Patient record deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while deleting the patient: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Retrieves the medical history records for a specific patient.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <returns>A list of medical history records.</returns>
        [HttpGet("{id}/medical-history")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> GetMedicalHistory(int id)
        {
            try
            {
                var histories = await _patientService.GetMedicalHistoryByPatientIdAsync(id);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Medical history retrieved successfully.",
                    Data = histories
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving medical history: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Adds a medical history diagnostic record for a patient.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <param name="history">The medical history details.</param>
        /// <returns>The created medical history entry.</returns>
        [HttpPost("{id}/medical-history")]
        [ProducesResponseType(typeof(ApiResponse), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> AddMedicalHistory(int id, [FromBody] MedicalHistory history)
        {
            if (id != history.PatientId)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Patient ID mismatch."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid medical history model."
                });
            }

            try
            {
                var success = await _patientService.AddMedicalHistoryAsync(history);
                if (!success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Failed to add medical history record."
                    });
                }

                return StatusCode(201, new ApiResponse
                {
                    Success = true,
                    Message = "Medical history record added successfully.",
                    Data = history
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while saving the medical history record: {ex.Message}"
                });
            }
        }
    }
}

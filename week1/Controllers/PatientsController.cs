using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using week1.Models;
using week1.Services;

namespace week1.Controllers
{
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

        // GET: api/patients?search=xyz
        [HttpGet]
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

        // GET: api/patients/5
        [HttpGet("{id}")]
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

        // POST: api/patients
        [HttpPost]
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

        // PUT: api/patients/5
        [HttpPut("{id}")]
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
    }
}

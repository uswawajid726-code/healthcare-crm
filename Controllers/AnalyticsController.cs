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
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("patients")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetPatientAnalytics()
        {
            try
            {
                var data = await _analyticsService.GetPatientAnalyticsAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Patient analytics retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving patient analytics: {ex.Message}"
                });
            }
        }

        [HttpGet("appointments")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetAppointmentAnalytics()
        {
            try
            {
                var data = await _analyticsService.GetAppointmentAnalyticsAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Appointment analytics retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving appointment analytics: {ex.Message}"
                });
            }
        }

        [HttpGet("doctors")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> GetDoctorAnalytics()
        {
            try
            {
                var data = await _analyticsService.GetDoctorAnalyticsAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Doctor analytics retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving doctor analytics: {ex.Message}"
                });
            }
        }
    }
}
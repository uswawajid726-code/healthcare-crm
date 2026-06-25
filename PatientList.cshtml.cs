using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HospitalApp.Pages.Patients
{
    [Authorize]
    public class PatientListModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public PatientListModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public List<PatientDto> Patients { get; set; } = new();
        public string? SearchQuery { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(string? search)
        {
            SearchQuery = search?.Trim();

            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Cookies["jwt_token"]
                            ?? HttpContext.Session.GetString("jwt_token");

                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                var baseUrl = _config["ApiBaseUrl"] ?? "https://localhost:7001";
                var url = string.IsNullOrWhiteSpace(SearchQuery)
                    ? $"{baseUrl}/api/patients"
                    : $"{baseUrl}/api/patients?search={Uri.EscapeDataString(SearchQuery)}";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Patients = JsonSerializer.Deserialize<List<PatientDto>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new List<PatientDto>();
                }
                else
                {
                    ErrorMessage = "Could not load patients. Please try again.";
                }
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Network error. Check your connection.";
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Cookies["jwt_token"]
                            ?? HttpContext.Session.GetString("jwt_token");

                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                var baseUrl = _config["ApiBaseUrl"] ?? "https://localhost:7001";
                var response = await client.DeleteAsync($"{baseUrl}/api/patients/{id}");

                if (!response.IsSuccessStatusCode)
                    TempData["Error"] = "Failed to delete patient.";
            }
            catch
            {
                TempData["Error"] = "Network error during delete.";
            }

            return RedirectToPage();
        }
    }

    // DTO matching the backend response
    public class PatientDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Cnic { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
    }
}

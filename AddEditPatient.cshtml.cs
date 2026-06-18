using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HospitalApp.Pages.Patients
{
    [Authorize]
    public class AddEditPatientModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public AddEditPatientModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public int? PatientId { get; set; }

        [BindProperty]
        public PatientInputModel Input { get; set; } = new();

        public bool IsLoading { get; set; }

        // GET — load existing patient if editing
        public async Task<IActionResult> OnGetAsync()
        {
            if (PatientId.HasValue)
            {
                IsLoading = true;
                var client = GetAuthorizedClient();
                var baseUrl = _config["ApiBaseUrl"] ?? "https://localhost:7001";

                var response = await client.GetAsync($"{baseUrl}/api/patients/{PatientId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var patient = JsonSerializer.Deserialize<PatientInputModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (patient != null)
                        Input = patient;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                IsLoading = false;
            }

            return Page();
        }

        // POST — create or update
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = GetAuthorizedClient();
            var baseUrl = _config["ApiBaseUrl"] ?? "https://localhost:7001";

            var payload = JsonSerializer.Serialize(Input);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (PatientId.HasValue)
            {
                // PUT — update existing
                response = await client.PutAsync($"{baseUrl}/api/patients/{PatientId}", content);
            }
            else
            {
                // POST — create new
                response = await client.PostAsync($"{baseUrl}/api/patients", content);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = PatientId.HasValue
                    ? "Patient updated successfully."
                    : "Patient added successfully.";
                return RedirectToPage("PatientList");
            }

            // Handle validation errors from API
            var errorJson = await response.Content.ReadAsStringAsync();
            try
            {
                var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(errorJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (errors != null)
                {
                    foreach (var (field, messages) in errors)
                        foreach (var msg in messages)
                            ModelState.AddModelError($"Input.{field}", msg);
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
            }

            return Page();
        }

        // Helper: attach JWT token to HTTP client
        private HttpClient GetAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = Request.Cookies["jwt_token"]
                        ?? HttpContext.Session.GetString("jwt_token");

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }

    // Input model with data annotations for server-side validation
    public class PatientInputModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "CNIC is required.")]
        [RegularExpression(@"^\d{5}-\d{7}-\d{1}$", ErrorMessage = "CNIC format: 42101-1234567-1")]
        public string Cnic { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone must start with 03 and be 11 digits.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Address { get; set; }

        [Display(Name = "Blood Group")]
        public string? BloodGroup { get; set; }
    }
}

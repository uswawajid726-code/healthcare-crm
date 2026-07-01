using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApp.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty; // Male / Female / Other

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone must start with 03 and be 11 digits.")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(5)]
        public string? BloodType { get; set; } // A+, A-, B+, B-, AB+, AB-, O+, O-

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active / Inactive

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Calculated, not stored in DB
        [NotMapped]
        public int Age =>
            DateTime.Today.Year - DateOfBirth.Year -
            (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

        // Navigation property
        public List<MedicalHistory>? MedicalHistories { get; set; }
    }

    public class MedicalHistory
    {
        [Key]
        public int MedicalHistoryId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Condition is required.")]
        [StringLength(200)]
        public string Condition { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DiagnosedDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(300)]
        public string? Medications { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }
    }
}

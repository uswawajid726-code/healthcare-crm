using System;
using System.ComponentModel.DataAnnotations;

namespace week1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Patient"; // e.g., Admin, Doctor, Patient

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

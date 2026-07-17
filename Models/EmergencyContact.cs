using System;
using System.ComponentModel.DataAnnotations;

namespace week1.Models
{
    public class EmergencyContact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Relationship { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
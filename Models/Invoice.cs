namespace week1.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class Invoice
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    [ForeignKey("AppointmentId")]
    public virtual Appointment? Appointment { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = "Unpaid";

    public DateTime CreatedAt { get; set; }
}
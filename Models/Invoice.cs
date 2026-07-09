namespace week1.Models;

public class Invoice
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = "Unpaid";

    public DateTime CreatedAt { get; set; }
}
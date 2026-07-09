namespace week1.Models;

public class Payment
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public DateTime PaidAt { get; set; }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week1.Data;
using week1.Models;

namespace week1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoicesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/invoices?status=Paid
        [HttpGet]
        public async Task<IActionResult> GetInvoices(string? status)
        {
            var invoices = _context.Invoices.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                invoices = invoices.Where(i => i.Status == status);
            }

            return Ok(await invoices.ToListAsync());
        }

        // POST: api/invoices
        [HttpPost]
        public async Task<IActionResult> CreateInvoice(Invoice invoice)
        {
            invoice.CreatedAt = DateTime.Now;
            invoice.Status = "Unpaid";

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        // PATCH: api/invoices/{id}/pay
        [HttpPatch("{id}/pay")]
        public async Task<IActionResult> PayInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            invoice.Status = "Paid";

            _context.Payments.Add(new Payment
            {
                InvoiceId = id,
                PaidAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(invoice);
        }
    }
}
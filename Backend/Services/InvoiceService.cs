using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Data;

namespace DKSKMaui.Backend.Services;

public class InvoiceService
{
    private readonly AppDbContext _context;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(AppDbContext context, ILogger<InvoiceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public virtual async Task<Invoice> AddInvoiceAsync(Invoice invoice)
    {
        try
        {
            _context.Invoice.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding invoice");
            throw;
        }
    }

    public virtual async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await _context.Invoice
                .Where(i => i.WorkDate >= startDate && i.WorkDate <= endDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting invoices by date range");
            return new List<Invoice>();
        }
    }

    public virtual async Task<List<Invoice>> GetInvoicesReceivable()
    {
        try
        {
            return await _context.Invoice
                .Where(i => i.AmountCost > (i.AmountPaid1 + i.AmountPaid2))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting receivable invoices");
            return new List<Invoice>();
        }
    }

    public virtual async Task<List<Invoice>> GetAllInvoicesAsync()
    {
        try
        {
            return await _context.Invoice.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting all invoices");
            return new List<Invoice>();
        }
    }

    public virtual async Task<bool> UpdateInvoiceAsync(Invoice invoice)
    {
        try
        {
            _context.Invoice.Update(invoice);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating invoice");
            return false;
        }
    }

    public virtual async Task<bool> DeleteInvoiceAsync(int invoiceId)
    {
        try
        {
            var invoice = await _context.Invoice.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.Invoice.Remove(invoice);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting invoice");
            return false;
        }
    }

    // Additional methods expected by components
    public virtual async Task<List<Invoice>> GetInvoicesSales()
    {
        try
        {
            return await _context.Invoice
                .Where(i => i.Status == 1) // Assuming status 1 means sales
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting sales invoices");
            return new List<Invoice>();
        }
    }

    public virtual async Task<List<Invoice>> GetInvoicesActive()
    {
        try
        {
            return await _context.Invoice
                .Where(i => i.Status == 0) // Assuming status 0 means active
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting active invoices");
            return new List<Invoice>();
        }
    }

    public virtual async Task<bool> UpdateInvoicesAsync(List<Invoice> invoices)
    {
        try
        {
            _context.Invoice.UpdateRange(invoices);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating invoices");
            return false;
        }
    }
}

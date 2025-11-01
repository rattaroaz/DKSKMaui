using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Data;

namespace DKSKMaui.Backend.Services;

public class CompanyService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(AppDbContext context, ILogger<CompanyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public virtual async Task<List<Companny>> GetAllCompaniesAsync()
    {
        try
        {
            return await _context.Companny
                .Include(c => c.Supervisors)
                    .ThenInclude(s => s.Properties)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting companies");
            return new List<Companny>();
        }
    }
    public virtual async Task<bool> SaveCompanyAsync(Companny company)
    {
        try
        {
            if (company.Id == 0)
            {
                // Create new company
                _context.Companny.Add(company);
            }
            else
            {
                // Update existing company
                _context.Companny.Update(company);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving company");
            return false;
        }
    }
    public virtual async Task<bool> DeleteCompanyAsync(int companyId)
    {
        try
        {
            var company = await _context.Companny.FindAsync(companyId);
            if (company != null)
            {
                _context.Companny.Remove(company);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting company");
            return false;
        }
    }

    public Companny GetCompanyInfo(Invoice invoice, List<Companny> compannies)
    {
        foreach (Companny companny in compannies)
        {
            if (invoice.CompanyName == companny.Name) return companny;
        }
        return new Companny { Name = invoice.CompanyName };
    }
    public Supervisor GetSupervisorInfo(Invoice invoice, List<Companny> compannies)
    {
        foreach (Companny companny in compannies)
        {
            if (invoice.CompanyName == companny.Name)
            {
                foreach (Supervisor supervisor in companny.Supervisors)
                {
                    foreach (Properties properties in supervisor.Properties)
                    {
                        if (properties.Address == invoice.PropertyAddress) return supervisor;
                    }
                }
            }
        }
        return new Supervisor { Name = invoice.CompanyName };
    }
    public Properties GetPropertyInfo(Invoice invoice, List<Companny> compannies)
    {
        foreach (Companny companny in compannies)
        {
            if (invoice.CompanyName == companny.Name)
            {
                foreach (Supervisor supervisor in companny.Supervisors)
                {
                    foreach (Properties properties in supervisor.Properties)
                    {
                        if (properties.Address == invoice.PropertyAddress) return properties;
                    }
                }
            }
        }
        return new Properties { Name = invoice.CompanyName };
    }
}

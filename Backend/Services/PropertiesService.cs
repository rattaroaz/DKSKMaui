using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Data;

namespace DKSKMaui.Backend.Services;

public class PropertiesService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PropertiesService> _logger;

    public PropertiesService(AppDbContext context, ILogger<PropertiesService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public virtual async Task<List<Properties>> GetAllPropertiesAsync()
    {
        try
        {
            return await _context.Properties.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting properties");
            return new List<Properties>();
        }
    }

    public virtual async Task<List<Properties>> GetPropertiesByCompanyIdAsync(int companyId)
    {
        try
        {
            return await _context.Properties
                .Include(p => p.Supervisor)
                .Where(p => p.Supervisor.CompanyId == companyId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting properties by company ID");
            return new List<Properties>();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Data;

namespace DKSKMaui.Backend.Services;

public class SupervisorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SupervisorService> _logger;

    public SupervisorService(AppDbContext context, ILogger<SupervisorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public virtual async Task<List<Supervisor>> GetAllSupervisorsAsync()
    {
        try
        {
            return await _context.Supervisor
                .Include(s => s.Company)
                .Include(s => s.Properties)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting supervisors");
            return new List<Supervisor>();
        }
    }
}

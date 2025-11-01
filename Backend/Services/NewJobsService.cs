using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Data;

namespace DKSKMaui.Backend.Services;

public class JobDescriptionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<JobDescriptionService> _logger;

    public JobDescriptionService(AppDbContext context, ILogger<JobDescriptionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public virtual async Task<List<JobDiscription>> GetAllJobsAsync()
    {
        try
        {
            return await _context.JobDiscription.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting job descriptions");
            return new List<JobDiscription>();
        }
    }

    public virtual async Task<bool> ReplaceAll(List<JobDiscription> jobs)
    {
        try
        {
            // Remove all existing job descriptions
            var existing = await _context.JobDiscription.ToListAsync();
            _context.JobDiscription.RemoveRange(existing);
            
            // Add new job descriptions
            _context.JobDiscription.AddRange(jobs);
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while replacing job descriptions");
            return false;
        }
    }
}

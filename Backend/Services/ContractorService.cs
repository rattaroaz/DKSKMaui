using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Data;

namespace DKSKMaui.Backend.Services;

public class ContractorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ContractorService> _logger;

    public ContractorService(AppDbContext context, ILogger<ContractorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public virtual async Task<List<Contractor>> GetAllContractorsAsync()
    {
        try
        {
            return await _context.Contractor.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting contractors");
            return new List<Contractor>();
        }
    }

    public virtual async Task<bool> SaveContractorAsync(Contractor contractor)
    {
        try
        {
            if (contractor.Id == 0)
            {
                // Check if a contractor with the same name already exists
                var existingContractor = await _context.Contractor
                    .FirstOrDefaultAsync(c => c.Name == contractor.Name);
                if (existingContractor != null)
                {
                    _logger.LogWarning("Attempted to create a contractor with a duplicate name: {Name}", contractor.Name);
                    return false; // Indicate failure due to duplicate
                }
                // Create new contractor
                _context.Contractor.Add(contractor);
            }
            else
            {
                // For updates, check if another contractor has the same name (excluding the current one)
                var existingContractor = await _context.Contractor
                    .FirstOrDefaultAsync(c => c.Name == contractor.Name && c.Id != contractor.Id);
                if (existingContractor != null)
                {
                    _logger.LogWarning("Attempted to update a contractor to a duplicate name: {Name}", contractor.Name);
                    return false; // Indicate failure due to duplicate
                }
                // Update existing contractor
                _context.Contractor.Update(contractor);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving contractor");
            return false;
        }
    }

    public virtual async Task<bool> DeleteContractorAsync(int contractorId)
    {
        try
        {
            var contractor = await _context.Contractor.FindAsync(contractorId);
            if (contractor != null)
            {
                _context.Contractor.Remove(contractor);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting contractor");
            return false;
        }
    }

    // Alias method expected by components
    public virtual async Task<bool> DeleteAsync(int contractorId)
    {
        return await DeleteContractorAsync(contractorId);
    }
}

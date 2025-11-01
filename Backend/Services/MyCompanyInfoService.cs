using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Data;

namespace DKSKMaui.Backend.Services;

public class MyCompanyInfoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MyCompanyInfoService> _logger;

    public MyCompanyInfoService(AppDbContext context, ILogger<MyCompanyInfoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public virtual async Task<MyCompanyInfo?> GetInfoAsync()
    {
        try
        {
            return await _context.MyCompanyInfo.FirstOrDefaultAsync() ?? new MyCompanyInfo();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting company info");
            return null;
        }
    }

    public virtual async Task UpdateInfoAsync(MyCompanyInfo model)
    {
        try
        {
            var existing = await _context.MyCompanyInfo.FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.Name = model.Name;
                existing.Phone = model.Phone;
                existing.Email = model.Email;
                existing.Address = model.Address;
                existing.LicenseNumber = model.LicenseNumber;
                existing.Zip = model.Zip;
                _context.MyCompanyInfo.Update(existing);
            }
            else
            {
                _context.MyCompanyInfo.Add(model);
            }
            
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating company info");
            throw;
        }
    }
}

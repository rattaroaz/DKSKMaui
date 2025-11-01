using Microsoft.EntityFrameworkCore;
using DKSKMaui.Backend.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DKSKMaui.Tests.Infrastructure;

/// <summary>
/// Base class for all unit tests providing common test infrastructure
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly AppDbContext DbContext;
    protected readonly IServiceProvider ServiceProvider;
    private bool _disposed;

    protected TestBase()
    {
        // Reset ID counter for test isolation
        TestDataBuilder.ResetIdCounter();
        
        // Create a unique database name for each test to ensure isolation
        var dbName = $"TestDb_{Guid.NewGuid()}";
        
        // Set up in-memory database with EnsureDeleted to clear any existing data
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection, dbName);
        
        ServiceProvider = serviceCollection.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Delete and recreate to ensure clean state
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
        
        // Clear any seeded data for test isolation
        ClearSeededData();
        
        // Clear any tracked entities
        DbContext.ChangeTracker.Clear();
        
        // Don't seed default data - tests should add their own
    }

    /// <summary>
    /// Clear all seeded data to ensure test isolation
    /// </summary>
    private void ClearSeededData()
    {
        DbContext.Properties.RemoveRange(DbContext.Properties);
        DbContext.Supervisor.RemoveRange(DbContext.Supervisor);
        DbContext.Companny.RemoveRange(DbContext.Companny);
        DbContext.Contractor.RemoveRange(DbContext.Contractor);
        DbContext.JobDiscription.RemoveRange(DbContext.JobDiscription);
        DbContext.MyCompanyInfo.RemoveRange(DbContext.MyCompanyInfo);
        DbContext.Invoice.RemoveRange(DbContext.Invoice);
        DbContext.SaveChanges();
    }

    /// <summary>
    /// Configure services for testing
    /// </summary>
    protected virtual void ConfigureServices(ServiceCollection services, string dbName)
    {
        // Add in-memory database
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName)
                   .EnableSensitiveDataLogging()
                   .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning)));

        // Add application services
        services.AddScoped<Backend.Services.CompanyService>();
        services.AddScoped<Backend.Services.ContractorService>();
        services.AddScoped<Backend.Services.InvoiceService>();
        services.AddScoped<Backend.Services.PropertiesService>();
        services.AddScoped<Backend.Services.SupervisorService>();
        services.AddScoped<Backend.Services.MyCompanyInfoService>();
        services.AddScoped<Backend.Services.JobDescriptionService>();
        services.AddScoped<Backend.Services.GlobalStateService>();
        
        // Add logging
        services.AddLogging();
    }

    /// <summary>
    /// Helper method to save changes and clear change tracker
    /// </summary>
    protected async Task SaveChangesAndClearTracking()
    {
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Gets the next available ID for a given entity type to avoid conflicts
    /// </summary>
    protected async Task<int> GetNextIdAsync<T>() where T : class
    {
        var dbSet = DbContext.Set<T>();
        return await dbSet.AnyAsync() ? await dbSet.MaxAsync(e => EF.Property<int>(e, "Id")) + 1 : 1;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DbContext?.Dispose();
                if (ServiceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _disposed = true;
        }
    }
}

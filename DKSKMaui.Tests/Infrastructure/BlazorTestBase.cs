using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bunit;
using Bunit.TestDoubles;
using DKSKMaui.Backend.Data;
using Radzen;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DKSKMaui.Tests.Infrastructure;

/// <summary>
/// </summary>
public abstract class BlazorTestBase : TestContext
{
    private AppDbContext? _dbContext;
    protected AppDbContext DbContext
    {
        get
        {
            if (_dbContext == null)
            {
                _dbContext = Services.GetRequiredService<AppDbContext>();
                // Delete and recreate to ensure clean state
                _dbContext.Database.EnsureDeleted();
                _dbContext.Database.EnsureCreated();
                // Clear any tracked entities
                _dbContext.ChangeTracker.Clear();
            }
            return _dbContext;
        }
    }

    protected BlazorTestBase()
    {
        // Reset ID counter for test isolation
        TestDataBuilder.ResetIdCounter();
        
        // Configure services for Blazor component testing
        ConfigureServices(Services);
        
        // Configure JSRuntime mock for Radzen components
        JSInterop.SetupVoid("Radzen.focusElement", _ => true);
        JSInterop.SetupVoid("Radzen.selectElement", _ => true);
        JSInterop.SetupVoid("Radzen.closePopup", _ => true);
        JSInterop.SetupVoid("Radzen.openPopup", _ => true);
        JSInterop.SetupVoid("Radzen.addCss", _ => true);
        JSInterop.SetupVoid("Radzen.removeCss", _ => true);
        JSInterop.SetupVoid("Radzen.setBodyCss", _ => true);
        JSInterop.SetupVoid("Radzen.preventArrows", _ => true);
        JSInterop.SetupVoid("Radzen.preventDefault", _ => true);
        JSInterop.SetupVoid("Radzen.stopPropagation", _ => true);
        JSInterop.SetupVoid("Radzen.createDatePicker", _ => true);
        JSInterop.SetupVoid("Radzen.destroyDatePicker", _ => true);
        JSInterop.SetupVoid("Radzen.updateDatePicker", _ => true);
        JSInterop.SetupVoid("Radzen.createDropDown", _ => true);
        JSInterop.SetupVoid("Radzen.destroyDropDown", _ => true);
        JSInterop.SetupVoid("Radzen.updateDropDown", _ => true);
        JSInterop.SetupVoid("Radzen.createDataGrid", _ => true);
        JSInterop.SetupVoid("Radzen.destroyDataGrid", _ => true);
        JSInterop.SetupVoid("Radzen.updateDataGrid", _ => true);
        
        // Don't retrieve DbContext here - wait until it's actually needed
    }

    /// <summary>
    /// Configure services for Blazor component testing
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        var dbName = $"BlazorTestDb_{Guid.NewGuid()}";
        
        // Add in-memory database
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        // Add Radzen services
        services.AddScoped<DialogService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<TooltipService>();
        services.AddScoped<ContextMenuService>();

        // Add application services
        services.AddScoped<Backend.Services.CompanyService>();
        services.AddScoped<Backend.Services.ContractorService>();
        services.AddScoped<Backend.Services.InvoiceService>();
        services.AddScoped<Backend.Services.PropertiesService>();
        services.AddScoped<Backend.Services.SupervisorService>();
        services.AddScoped<Backend.Services.MyCompanyInfoService>();
        services.AddScoped<Backend.Services.JobDescriptionService>();
        services.AddScoped<Backend.Services.GlobalStateService>();

        // Add navigation manager using bUnit's FakeNavigationManager
        services.AddSingleton<NavigationManager, FakeNavigationManager>();
        
        // Add logging
        services.AddLogging();
    }

    /// <summary>
    /// Gets the next available ID for a given entity type to avoid conflicts
    /// </summary>
    protected async Task<int> GetNextIdAsync<T>() where T : class
    {
        var dbSet = DbContext.Set<T>();
        return await dbSet.AnyAsync() ? await dbSet.MaxAsync(e => EF.Property<int>(e, "Id")) + 1 : 1;
    }
    protected new IRenderedComponent<TComponent> RenderComponent<TComponent>(
        params ComponentParameter[] parameters) where TComponent : Microsoft.AspNetCore.Components.IComponent
    {
        return base.RenderComponent<TComponent>(parameters);
    }

    /// <summary>
    /// Helper method to render a component with builder
    /// </summary>
    protected new IRenderedComponent<TComponent> RenderComponent<TComponent>(
        Action<ComponentParameterCollectionBuilder<TComponent>> parameterBuilder) 
        where TComponent : Microsoft.AspNetCore.Components.IComponent
    {
        return base.RenderComponent<TComponent>(parameterBuilder);
    }

    protected override void Dispose(bool disposing)
    {
        _dbContext?.Dispose();
        base.Dispose(disposing);
    }
}

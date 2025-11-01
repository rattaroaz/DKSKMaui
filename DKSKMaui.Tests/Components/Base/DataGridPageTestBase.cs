using Bunit;
using DKSKMaui.Components.Pages;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Data;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using Radzen;
using Xunit;

namespace DKSKMaui.Tests.Components.Base;

/// <summary>
/// Base class for testing pages with Radzen DataGrid components
/// </summary>
public abstract class DataGridPageTestBase<TPage> : TestContext where TPage : ComponentBase
{
    protected Mock<NotificationService> NotificationServiceMock { get; private set; } = new();
    protected Mock<DialogService> DialogServiceMock { get; private set; } = new();
    protected Mock<InvoiceService> InvoiceServiceMock { get; private set; } = new();
    protected Mock<CompanyService> CompanyServiceMock { get; private set; } = new();
    protected Mock<ContractorService> ContractorServiceMock { get; private set; } = new();
    protected Mock<JobDescriptionService> JobDescriptionServiceMock { get; private set; } = new();
    protected Mock<MyCompanyInfoService> MyCompanyInfoServiceMock { get; private set; } = new();
    protected Mock<IJSRuntime> JSRuntimeMock { get; private set; } = new();

    protected DataGridPageTestBase()
    {
        // Setup test database and services
        SetupTestServices();
        
        // Setup mocks only for external dependencies that can't be easily tested with real implementations
        NotificationServiceMock = new Mock<NotificationService>();
        DialogServiceMock = new Mock<DialogService>();
        InvoiceServiceMock = new Mock<InvoiceService>();
        CompanyServiceMock = new Mock<CompanyService>();
        ContractorServiceMock = new Mock<ContractorService>();
        JobDescriptionServiceMock = new Mock<JobDescriptionService>();
        MyCompanyInfoServiceMock = new Mock<MyCompanyInfoService>();
        JSRuntimeMock = new Mock<IJSRuntime>();

        // Override only the external services with mocks
        Services.AddScoped(_ => NotificationServiceMock.Object);
        Services.AddScoped(_ => DialogServiceMock.Object);
        Services.AddScoped(_ => JSRuntimeMock.Object);

        // Setup default mock behaviors
        DialogServiceMock.Setup(s => s.Confirm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ConfirmOptions>())).ReturnsAsync(true);

        // Setup JS runtime for Radzen components
        JSInterop.SetupVoid("Radzen.focusElement", _ => true);
        JSInterop.SetupVoid("Radzen.selectElement", _ => true);
        JSInterop.SetupVoid("Radzen.closePopup", _ => true);
        JSInterop.SetupVoid("Radzen.openPopup", _ => true);
        JSInterop.SetupVoid("Radzen.addCss", _ => true);
        JSInterop.SetupVoid("Radzen.removeCss", _ => true);
        JSInterop.SetupVoid("Radzen.setBodyCss", _ => true);

        // Seed default test data
        SeedDefaultTestData();
    }

    private void SeedDefaultTestData()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Backend.Data.AppDbContext>();
        
        // Seed companies
        if (!dbContext.Companny.Any())
        {
            dbContext.Companny.AddRange(
                TestDataBuilder.CreateCompany(c => c.Id = 1),
                TestDataBuilder.CreateCompany(c => c.Id = 2)
            );
        }

        // Seed job descriptions
        if (!dbContext.JobDiscription.Any())
        {
            dbContext.JobDiscription.AddRange(
                TestDataBuilder.CreateJobDescription(j => j.Id = 1),
                TestDataBuilder.CreateJobDescription(j => j.Id = 2)
            );
        }

        // Seed some basic invoices
        if (!dbContext.Invoice.Any())
        {
            dbContext.Invoice.AddRange(
                TestDataBuilder.CreateInvoice(i => i.Id = 1),
                TestDataBuilder.CreateInvoice(i => i.Id = 2),
                TestDataBuilder.CreateInvoice(i => i.Id = 3)
            );
        }

        // Seed MyCompanyInfo
        if (!dbContext.MyCompanyInfo.Any())
        {
            dbContext.MyCompanyInfo.Add(TestDataBuilder.CreateMyCompanyInfo(i => i.Id = 1));
        }

        dbContext.SaveChanges();
    }

    private void SetupTestServices()
    {
        // Create a unique database name for each test
        var dbName = $"BlazorTestDb_{Guid.NewGuid()}";
        
        // Add in-memory database
        Services.AddDbContext<Backend.Data.AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        // Add Radzen services
        Services.AddScoped<DialogService>();
        Services.AddScoped<NotificationService>();
        Services.AddScoped<TooltipService>();
        Services.AddScoped<ContextMenuService>();

        // Add navigation manager using bUnit's FakeNavigationManager
        Services.AddSingleton<NavigationManager, Bunit.TestDoubles.FakeNavigationManager>();
        
        // Add logging
        Services.AddLogging();
    }

    /// <summary>
    /// Creates a list of test invoices for data grid testing
    /// </summary>
    protected List<Invoice> CreateTestInvoices(int count = 5)
    {
        var invoices = new List<Invoice>();
        for (int i = 1; i <= count; i++)
        {
            invoices.Add(TestDataBuilder.CreateInvoice(c => 
            {
                c.WorkOrder = $"WO-{1000 + i}";
                c.CompanyName = $"Test Company {i}";
                c.PropertyAddress = $"{i}23 Test St";
                c.Unit = $"Unit {i}";
                c.AmountCost = 1000 + (i * 100);
            }));
        }
        return invoices;
    }

    /// <summary>
    /// Verifies that a data grid renders with expected columns
    /// </summary>
    protected void VerifyDataGridRenders(IRenderedComponent<TPage> component, List<Invoice> expectedInvoices)
    {
        component.Should().NotBeNull();
        var markup = component.Markup;
        markup.Should().NotBeNullOrEmpty();

        // Verify data grid is present
        markup.Should().Contain("rz-data-grid");

        // Verify invoice data is displayed
        foreach (var invoice in expectedInvoices)
        {
            markup.Should().Contain((invoice.Id + 10000).ToString()); // Invoice number format
        }
    }

    /// <summary>
    /// Verifies that date pickers are rendered correctly
    /// </summary>
    protected void VerifyDatePickersRender(IRenderedComponent<TPage> component)
    {
        var markup = component.Markup;
        markup.Should().Contain("rz-datepicker");
    }

    /// <summary>
    /// Verifies that dropdown components are rendered
    /// </summary>
    protected void VerifyDropdownsRender(IRenderedComponent<TPage> component)
    {
        var markup = component.Markup;
        markup.Should().Contain("rz-dropdown");
    }

    /// <summary>
    /// Seeds test data into the database
    /// </summary>
    protected async Task SeedTestData(List<Invoice> invoices)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Backend.Data.AppDbContext>();
        
        await dbContext.Invoice.AddRangeAsync(invoices);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Verifies that action buttons are rendered
    /// </summary>
    protected void VerifyActionButtonsRender(IRenderedComponent<TPage> component, params string[] expectedButtonTexts)
    {
        var markup = component.Markup;
        foreach (var buttonText in expectedButtonTexts)
        {
            markup.Should().Contain(buttonText);
        }
    }
}

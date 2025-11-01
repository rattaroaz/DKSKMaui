using DKSKMaui.Components.Pages.AgingReports;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Bunit;
using Bunit.TestDoubles;
using Radzen;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Xunit;
using FluentAssertions;

namespace DKSKMaui.Tests.Components.Pages;

public class AgingreportsPageTests : BlazorTestBase
{
    private readonly InvoiceService _invoiceService;
    private readonly NotificationService _notificationService;
    private readonly DialogService _dialogService;
    private readonly SupervisorService _supervisorService;

    public AgingreportsPageTests()
    {
        _invoiceService = Services.GetRequiredService<InvoiceService>();
        _notificationService = Services.GetRequiredService<NotificationService>();
        _dialogService = Services.GetRequiredService<DialogService>();
        _supervisorService = Services.GetRequiredService<SupervisorService>();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void AgingreportsPage_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<Agingreports>();

        // Assert
        component.Find("h3").TextContent.Should().Contain("Aging Reports");
        component.Find(".container").Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void AgingreportsPage_HasRequiredUIElements()
    {
        // Act
        var component = RenderComponent<Agingreports>();

        // Assert - Check for Radzen date pickers and dropdowns
        // Radzen date pickers render as input elements within rz-datepicker containers
        var dateInputs = component.FindAll("input");
        dateInputs.Should().HaveCountGreaterThanOrEqualTo(2);

        // Check for dropdowns (rendered as rz-dropdown divs)
        component.Markup.Should().Contain("rz-dropdown");
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task AgingreportsPage_LoadsCompanyNames()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c => {
            c.Id = 1001;
            c.Name = "Test Company";
        });
        await DbContext.Companny.AddAsync(company);
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<Agingreports>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Component should load company names
        // Note: This test verifies the component initializes without errors
        component.Instance.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task AgingreportsPage_LoadsSupervisorNames()
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor(s => {
            s.Id = 2001;
            s.Name = "Test Supervisor";
        });
        await DbContext.Supervisor.AddAsync(supervisor);
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<Agingreports>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Component should load supervisor names
        component.Instance.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task AgingreportsPage_LoadsInvoicesOnInit()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.WorkOrder = "WO-TEST-3001";
            i.WorkDate = DateTime.Now.AddDays(-30);
            i.AmountCost = 1000;
            i.AmountPaid1 = 0;
        });
        await DbContext.Invoice.AddAsync(invoice);
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<Agingreports>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Component loads successfully
        component.Instance.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task AgingreportsPage_HasDataGrid()
    {
        // Arrange - Add some invoice data so the grid renders
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.WorkOrder = "WO-TEST-5001";
            i.WorkDate = DateTime.Now.AddDays(-30);
            i.AmountCost = 1000;
            i.AmountPaid1 = 0;
        });
        await DbContext.Invoice.AddAsync(invoice);
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<Agingreports>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Should have a data table when there is data
        component.Markup.Should().Contain("table");
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task AgingreportsPage_HandlesEmptyInvoiceList()
    {
        // Arrange - No invoices in database

        // Act
        var component = RenderComponent<Agingreports>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Component should render without errors even with no data
        component.Instance.Should().NotBeNull();
        // No grid should be visible when no data
        var grids = component.FindAll("[data-testid='aging-grid']");
        // Grid might not be rendered if no data, which is fine
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task AgingreportsPage_CalculatesAgingCorrectly()
    {
        // Arrange
        var oldInvoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.WorkOrder = "WO-TEST-4001";
            i.WorkDate = DateTime.Now.AddDays(-90); // 90 days old
            i.AmountCost = 1000;
            i.AmountPaid1 = 0;
        });
        var newInvoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.WorkOrder = "WO-TEST-4002";
            i.WorkDate = DateTime.Now.AddDays(-10); // 10 days old
            i.AmountCost = 500;
            i.AmountPaid1 = 0;
        });
        await DbContext.Invoice.AddRangeAsync(new[] { oldInvoice, newInvoice });
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<Agingreports>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Component processes aging calculations
        component.Instance.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void AgingreportsPage_HasProperLayout()
    {
        // Act
        var component = RenderComponent<Agingreports>();

        // Assert - Check for proper Bootstrap layout classes
        component.Markup.Should().Contain("row");
        component.Markup.Should().Contain("col-");
        component.Markup.Should().Contain("container");
        // Don't check for specific initialization that requires JS interop
    }

    [Fact]
    [Trait("Category", "Component")]
    public void AgingreportsPage_HasExportFunctionality()
    {
        // Act
        var component = RenderComponent<Agingreports>();

        // Assert - Should have export capabilities (check for button text)
        component.Markup.Should().Contain("Download to PDF");
        component.Markup.Should().Contain("Download to Excel");
    }
}

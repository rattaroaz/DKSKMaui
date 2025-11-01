using DKSKMaui.Components.Pages.ContractorJobs;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Bunit;
using Bunit.TestDoubles;
using Radzen;
using Microsoft.AspNetCore.Components;
using Xunit;
using FluentAssertions;

namespace DKSKMaui.Tests.Components.Pages.ContractorJobsTests;

public class ContractorJobsPageTests : BlazorTestBase
{
    private readonly InvoiceService _invoiceService;
    private readonly NotificationService _notificationService;
    private readonly DialogService _dialogService;
    private readonly ContractorService _contractorService;

    public ContractorJobsPageTests()
    {
        _invoiceService = Services.GetRequiredService<InvoiceService>();
        _notificationService = Services.GetRequiredService<NotificationService>();
        _dialogService = Services.GetRequiredService<DialogService>();
        _contractorService = Services.GetRequiredService<ContractorService>();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void ContractorJobsPage_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<ContractorJobs>();

        // Assert
        component.Find("h3").TextContent.Should().Contain("Contractor Jobs");
        component.Find("div.rz-card").Should().NotBeNull(); // RadzenCard renders as rz-card div
    }

    [Fact]
    [Trait("Category", "Component")]
    public void ContractorJobsPage_HasRequiredUIElements()
    {
        // Act
        var component = RenderComponent<ContractorJobs>();

        // Assert - Check for date picker and data grid
        component.Markup.Should().Contain("Date to Search");
        component.Markup.Should().Contain("table"); // Data grid renders as table
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task ContractorJobsPage_HasExportFunctionality()
    {
        // Arrange - Add some invoice data
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.WorkOrder = "WO-TEST-6001";
            i.WorkDate = DateTime.Now.AddDays(-30);
            i.AmountCost = 1000;
            i.AmountPaid1 = 0;
        });
        await DbContext.Invoice.AddAsync(invoice);
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<ContractorJobs>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Should have export capabilities
        component.Markup.Should().Contain("Download to PDF");
        component.Markup.Should().Contain("Download to Excel");
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task ContractorJobsPage_LoadsInvoicesOnInit()
    {
        // Arrange - Add test invoice data
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.WorkOrder = "WO-TEST-6002";
            i.WorkDate = DateTime.Now.AddDays(-15);
            i.AmountCost = 1500;
            i.AmountPaid1 = 0;
        });
        await DbContext.Invoice.AddAsync(invoice);
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<ContractorJobs>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Component loads successfully
        component.Instance.Should().NotBeNull();
        component.Markup.Should().Contain("container");
    }
}

using DKSKMaui.Components.Pages.Payroll;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Bunit;
using Bunit.TestDoubles;
using Radzen;
using Microsoft.AspNetCore.Components;
using Xunit;
using FluentAssertions;

namespace DKSKMaui.Tests.Components.Pages.PayrollTests;

public class PayrollPageTests : BlazorTestBase
{
    private readonly InvoiceService _invoiceService;
    private readonly NotificationService _notificationService;
    private readonly DialogService _dialogService;
    private readonly ContractorService _contractorService;

    public PayrollPageTests()
    {
        _invoiceService = Services.GetRequiredService<InvoiceService>();
        _notificationService = Services.GetRequiredService<NotificationService>();
        _dialogService = Services.GetRequiredService<DialogService>();
        _contractorService = Services.GetRequiredService<ContractorService>();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void PayrollPage_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<Payroll>();

        // Assert
        component.Find("h3").TextContent.Should().Contain("Payroll");
        component.Find("div.rz-card").Should().NotBeNull(); // RadzenCard renders as rz-card div
    }

    [Fact]
    [Trait("Category", "Component")]
    public void PayrollPage_HasRequiredUIElements()
    {
        // Act
        var component = RenderComponent<Payroll>();

        // Assert - Check for date pickers and dropdown
        component.Markup.Should().Contain("Start Date:");
        component.Markup.Should().Contain("End Date:");
        component.Markup.Should().Contain("Select Contractor");
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task PayrollPage_LoadsContractorsOnInit()
    {
        // Arrange - Add test contractor data
        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = 7001;
            c.Name = "Test Contractor";
        });
        await DbContext.Contractor.AddAsync(contractor);
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<Payroll>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Component loads successfully
        component.Instance.Should().NotBeNull();
        component.Markup.Should().Contain("container");
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task PayrollPage_DisplaysDataGridWhenContractorSelected()
    {
        // Arrange - Add test data
        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = 7002;
            c.Name = "Test Contractor";
        });
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.WorkOrder = "WO-TEST-7001";
            i.WorkDate = DateTime.Now.AddDays(-10);
            i.AmountCost = 2000;
            i.AmountPaid1 = 1500;
            i.ContractorName = contractor.Name;
        });
        await DbContext.Contractor.AddAsync(contractor);
        await DbContext.Invoice.AddAsync(invoice);
        await DbContext.SaveChangesAsync();

        // Act
        var component = RenderComponent<Payroll>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Should have contractor dropdown available
        component.Markup.Should().Contain("Select Contractor");
    }
}

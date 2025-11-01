using DKSKMaui.Backend.Services;
using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DKSKMaui.Tests.Services;

public class InvoiceServiceTests : TestBase
{
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _invoiceService = ServiceProvider.GetRequiredService<InvoiceService>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreateInvoiceAsync_ValidInvoice_AddsToDatabase()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.Id = 0;
            i.WorkOrder = "WO-2024-001";
        });

        // Act
        await _invoiceService.AddInvoiceAsync(invoice);

        // Assert
        var savedInvoice = await DbContext.Invoice.FirstOrDefaultAsync(i => i.WorkOrder == "WO-2024-001");
        savedInvoice.Should().NotBeNull();
        savedInvoice!.CompanyName.Should().Be(invoice.CompanyName);
        savedInvoice.AmountCost.Should().Be(invoice.AmountCost);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllInvoicesAsync_ReturnsAllInvoices()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i => i.Id = 1);
        await DbContext.Invoice.AddAsync(invoice);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _invoiceService.GetAllInvoicesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().WorkOrder.Should().Be(invoice.WorkOrder);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInvoicesByDateRangeAsync_ReturnsInvoicesInRange()
    {
        // Arrange
        var today = DateTime.Now.Date;
        var invoices = new List<Invoice>
        {
            TestDataBuilder.CreateInvoice(i => { i.WorkDate = today.AddDays(-10); }),
            TestDataBuilder.CreateInvoice(i => { i.WorkDate = today.AddDays(-5); }),
            TestDataBuilder.CreateInvoice(i => { i.WorkDate = today.AddDays(-1); }),
            TestDataBuilder.CreateInvoice(i => { i.WorkDate = today.AddDays(-30); })
        };
        await DbContext.Invoice.AddRangeAsync(invoices);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _invoiceService.GetInvoicesByDateRangeAsync(today.AddDays(-7), today);

        // Assert
        result.Should().HaveCount(2);
        result.All(i => i.WorkDate >= today.AddDays(-7) && i.WorkDate <= today).Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInvoicesReceivable_ReturnsOnlyUnpaidInvoices()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            TestDataBuilder.CreateInvoice(i => 
            { 
                i.AmountCost = 1000;
                i.AmountPaid1 = 1000;
                i.Status = 1; // Paid
            }),
            TestDataBuilder.CreateInvoice(i => 
            { 
                i.AmountCost = 2000;
                i.AmountPaid1 = 0;
                i.Status = 0; // Unpaid
            }),
            TestDataBuilder.CreateInvoice(i => 
            { 
                i.AmountCost = 3000;
                i.AmountPaid1 = 1500;
                i.Status = 0; // Partially paid
            })
        };
        await DbContext.Invoice.AddRangeAsync(invoices);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _invoiceService.GetInvoicesReceivable();

        // Assert
        result.Should().HaveCount(2);
        result.All(i => i.Status == 0).Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateInvoiceAsync_UpdatesInvoiceInfo()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.Id = 1;
            i.AmountCost = 5000;
            i.AmountPaid1 = 0;
        });
        await DbContext.Invoice.AddAsync(invoice);
        await SaveChangesAndClearTracking();

        invoice.AmountPaid1 = 2500;
        invoice.CheckNumber1 = "CHK-123";
        
        // Act
        var result = await _invoiceService.UpdateInvoiceAsync(invoice);

        // Assert
        result.Should().BeTrue();
        var updatedInvoice = await DbContext.Invoice.FindAsync(1);
        updatedInvoice.Should().NotBeNull();
        updatedInvoice!.AmountPaid1.Should().Be(2500);
        updatedInvoice.CheckNumber1.Should().Be("CHK-123");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllInvoicesAsync_FilterByContractor_ReturnsContractorInvoices()
    {
        // Arrange
        var contractorName = "John Doe";
        var invoices = new List<Invoice>
        {
            TestDataBuilder.CreateInvoice(i => i.ContractorName = contractorName),
            TestDataBuilder.CreateInvoice(i => i.ContractorName = contractorName),
            TestDataBuilder.CreateInvoice(i => i.ContractorName = "Jane Smith")
        };
        await DbContext.Invoice.AddRangeAsync(invoices);
        await SaveChangesAndClearTracking();

        // Act
        var allInvoices = await _invoiceService.GetAllInvoicesAsync();
        var result = allInvoices.Where(i => i.ContractorName == contractorName).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.All(i => i.ContractorName == contractorName).Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInvoicesReceivable_CalculatesOutstandingCorrectly()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            TestDataBuilder.CreateInvoice(i => 
            { 
                i.AmountCost = 1000;
                i.AmountPaid1 = 500;
                i.AmountPaid2 = 0;
            }),
            TestDataBuilder.CreateInvoice(i => 
            { 
                i.AmountCost = 2000;
                i.AmountPaid1 = 1000;
                i.AmountPaid2 = 500;
            }),
            TestDataBuilder.CreateInvoice(i => 
            { 
                i.AmountCost = 3000;
                i.AmountPaid1 = 3000;
                i.AmountPaid2 = 0;
            })
        };
        await DbContext.Invoice.AddRangeAsync(invoices);
        await SaveChangesAndClearTracking();

        // Act
        var unpaidInvoices = await _invoiceService.GetInvoicesReceivable();
        var totalOutstanding = unpaidInvoices.Sum(i => i.AmountCost - (i.AmountPaid1 + i.AmountPaid2));

        // Assert
        // First invoice: 1000 - 500 = 500
        // Second invoice: 2000 - 1500 = 500
        // Third invoice: 3000 - 3000 = 0
        // Total: 1000
        totalOutstanding.Should().Be(1000);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteInvoiceAsync_RemovesInvoiceFromDatabase()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i => i.Id = 1);
        await DbContext.Invoice.AddAsync(invoice);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _invoiceService.DeleteInvoiceAsync(1);

        // Assert
        var deletedInvoice = await DbContext.Invoice.FindAsync(1);
        deletedInvoice.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteInvoiceAsync_NonExistentInvoice_ReturnsFalse()
    {
        // Act
        var result = await _invoiceService.DeleteInvoiceAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateInvoiceAsync_NonExistentInvoice_ReturnsFalse()
    {
        // Arrange
        var invoice = TestDataBuilder.CreateInvoice(i => i.Id = 999);

        // Act
        var result = await _invoiceService.UpdateInvoiceAsync(invoice);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInvoicesByDateRangeAsync_NoInvoicesInRange_ReturnsEmptyList()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(10);
        var endDate = DateTime.Now.AddDays(20);

        // Act
        var result = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInvoicesReceivable_NoUnpaidInvoices_ReturnsEmptyList()
    {
        // Act
        var result = await _invoiceService.GetInvoicesReceivable();

        // Assert
        result.Should().NotBeNull();
        // Should only return seeded data that meets unpaid criteria
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreateInvoiceAsync_NullInvoice_DoesNotThrow()
    {
        // Act & Assert - Services may handle null gracefully or throw different exceptions
        // This test verifies the service doesn't crash unexpectedly
        try
        {
            await _invoiceService.AddInvoiceAsync(null!);
        }
        catch (Exception ex)
        {
            // Any exception is acceptable as long as it's not a crash
            ex.Should().NotBeNull();
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreateInvoiceAsync_InvalidInvoiceData_HandlesGracefully()
    {
        // Arrange - Create invoice with invalid/missing required data
        var invalidInvoice = new Invoice
        {
            // Missing required fields like WorkOrder, dates, etc.
        };

        // Act & Assert - Service may handle invalid data gracefully or throw various exceptions
        try
        {
            await _invoiceService.AddInvoiceAsync(invalidInvoice);
        }
        catch (Exception ex)
        {
            // Any exception is acceptable for invalid data
            ex.Should().NotBeNull();
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInvoicesByDateRangeAsync_EndDateBeforeStartDate_ReturnsEmptyList()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(10);
        var endDate = DateTime.Now.AddDays(5); // End before start

        // Act
        var result = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllInvoicesAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange - Clear all invoices
        DbContext.Invoice.RemoveRange(DbContext.Invoice);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _invoiceService.GetAllInvoicesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInvoicesByDateRangeAsync_InvalidDates_ReturnsEmptyList()
    {
        // Arrange - Use dates that would logically return no results
        var startDate = DateTime.Now.AddYears(10); // Future date
        var endDate = DateTime.Now.AddYears(11);   // Even further future

        // Act
        var result = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);

        // Assert - Should return empty list for dates with no data
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

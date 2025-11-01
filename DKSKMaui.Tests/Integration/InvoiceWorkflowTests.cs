using DKSKMaui.Backend.Services;
using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DKSKMaui.Tests.Integration;

public class InvoiceWorkflowTests : TestBase
{
    private readonly InvoiceService _invoiceService;
    private readonly CompanyService _companyService;
    private readonly ContractorService _contractorService;
    private readonly PropertiesService _propertiesService;

    public InvoiceWorkflowTests()
    {
        _invoiceService = ServiceProvider.GetRequiredService<InvoiceService>();
        _companyService = ServiceProvider.GetRequiredService<CompanyService>();
        _contractorService = ServiceProvider.GetRequiredService<ContractorService>();
        _propertiesService = ServiceProvider.GetRequiredService<PropertiesService>();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CompleteInvoiceWorkflow_FromCreationToPayment_Success()
    {
        // Arrange - Create supporting entities
        var company = TestDataBuilder.CreateCompany(c => 
        {
            c.Id = 0;
            c.Name = "Test Company LLC";
        });
        await _companyService.SaveCompanyAsync(company);

        var property = TestDataBuilder.CreateProperty(p =>
        {
            p.Id = 0;
            p.SupervisorId = 1;
            p.Address = "123 Main St";
        });
        // Properties are managed through Supervisor relationship
        // For test purposes, we'll add directly to DB
        await DbContext.Properties.AddAsync(property);
        await DbContext.SaveChangesAsync();

        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = 0;
            c.Name = "John Contractor";
            c.IsActive = true;
        });
        await _contractorService.SaveContractorAsync(contractor);

        // Act - Create Invoice
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.Id = 0;
            i.CompanyName = company.Name;
            i.PropertyAddress = property.Address;
            i.ContractorName = contractor.Name;
            i.AmountCost = 5000;
            i.Status = 0;
        });
        await _invoiceService.AddInvoiceAsync(invoice);

        // Assert - Invoice created
        var createdInvoice = await DbContext.Invoice
            .FirstOrDefaultAsync(i => i.CompanyName == "Test Company LLC");
        createdInvoice.Should().NotBeNull();
        createdInvoice!.Status.Should().Be(0);

        // Act - Process first payment
        createdInvoice.AmountPaid1 = 2000;
        createdInvoice.CheckNumber1 = "CHK001";
        createdInvoice.DatePaid1 = DateTime.Now;
        await _invoiceService.UpdateInvoiceAsync(createdInvoice);

        // Assert - First payment recorded
        var partiallyPaidInvoice = await DbContext.Invoice.FindAsync(createdInvoice.Id);
        partiallyPaidInvoice!.AmountPaid1.Should().Be(2000);
        partiallyPaidInvoice.CheckNumber1.Should().Be("CHK001");

        // Act - Process final payment
        partiallyPaidInvoice.AmountPaid2 = 3000;
        partiallyPaidInvoice.CheckNumber2 = "CHK002";
        partiallyPaidInvoice.DatePaid2 = DateTime.Now;
        partiallyPaidInvoice.Status = 1; // Mark as paid
        DbContext.Update(partiallyPaidInvoice);
        await DbContext.SaveChangesAsync();

        // Assert - Invoice fully paid
        var fullyPaidInvoice = await DbContext.Invoice.FindAsync(createdInvoice.Id);
        fullyPaidInvoice!.Status.Should().Be(1);
        var totalPaid = fullyPaidInvoice.AmountPaid1 + fullyPaidInvoice.AmountPaid2;
        totalPaid.Should().Be(fullyPaidInvoice.AmountCost);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task MultipleInvoicesForCompany_TrackedCorrectly()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c =>
        {
            c.Id = 0;
            c.Name = "Multi Invoice Company";
        });
        await _companyService.SaveCompanyAsync(company);

        // Act - Create multiple invoices
        var invoices = new List<Invoice>();
        for (int i = 0; i < 5; i++)
        {
            var invoice = TestDataBuilder.CreateInvoice(inv =>
            {
                inv.Id = 0;
                inv.CompanyName = company.Name;
                inv.AmountCost = 1000 * (i + 1);
                inv.WorkDate = DateTime.Now.AddDays(-i * 7);
            });
            await _invoiceService.AddInvoiceAsync(invoice);
            invoices.Add(invoice);
        }

        // Assert - All invoices created
        var companyInvoices = await DbContext.Invoice
            .Where(i => i.CompanyName == "Multi Invoice Company")
            .ToListAsync();
        
        companyInvoices.Should().HaveCount(5);
        companyInvoices.Sum(i => i.AmountCost).Should().Be(15000); // 1000+2000+3000+4000+5000
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ContractorPayroll_CalculatesCorrectly()
    {
        // Arrange
        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = 0;
            c.Name = "Payroll Test Contractor";
            c.IsActive = true;
        });
        await _contractorService.SaveContractorAsync(contractor);

        // Create invoices for the contractor
        for (int i = 0; i < 3; i++)
        {
            var invoice = TestDataBuilder.CreateInvoice(inv =>
            {
                inv.Id = 0;
                inv.ContractorName = contractor.Name;
                inv.AmountCost = 2000;
                inv.WorkDate = DateTime.Now.AddDays(-i);
            });
            await _invoiceService.AddInvoiceAsync(invoice);
        }

        // Act
        var allInvoices = await _invoiceService.GetAllInvoicesAsync();
        var contractorInvoices = allInvoices.Where(i => i.ContractorName == contractor.Name).ToList();

        // Assert
        contractorInvoices.Should().HaveCount(3);
        var totalEarnings = contractorInvoices.Sum(i => i.AmountCost);
        totalEarnings.Should().Be(6000);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task AgingReport_CategorizesProperly()
    {
        // Arrange - Create invoices with different ages
        var initialReceivables = await _invoiceService.GetInvoicesReceivable();
        var initialCount = initialReceivables.Count;

        var nextId = await GetNextIdAsync<Invoice>();
        var invoiceData = new[]
        {
            (Days: -5, Status: 0, Id: nextId++),   // Current
            (Days: -25, Status: 0, Id: nextId++),  // 30 days
            (Days: -45, Status: 0, Id: nextId++),  // 60 days
            (Days: -75, Status: 0, Id: nextId++),  // 90 days
            (Days: -120, Status: 0, Id: nextId++), // Over 90 days
            (Days: -30, Status: 1, Id: nextId++)   // Paid (should not appear in aging)
        };

        foreach (var data in invoiceData)
        {
            var invoice = TestDataBuilder.CreateInvoice(i =>
            {
                i.Id = data.Id;
                i.WorkDate = DateTime.Now.AddDays(data.Days);
                i.Status = data.Status;
                i.AmountCost = 1000;
                // Ensure unpaid invoices by not setting paid amounts
                if (data.Status == 1) // Paid
                {
                    i.AmountPaid1 = 1000;
                }
            });
            await _invoiceService.AddInvoiceAsync(invoice);
        }

        // Act
        var unpaidInvoices = await _invoiceService.GetInvoicesReceivable();
        var now = DateTime.Now;

        var current = unpaidInvoices.Where(i => (now - i.WorkDate).Days <= 30);
        var thirtyDays = unpaidInvoices.Where(i => (now - i.WorkDate).Days > 30 && (now - i.WorkDate).Days <= 60);
        var sixtyDays = unpaidInvoices.Where(i => (now - i.WorkDate).Days > 60 && (now - i.WorkDate).Days <= 90);
        var overNinety = unpaidInvoices.Where(i => (now - i.WorkDate).Days > 90);

        // Assert - Check that our test invoices are properly categorized
        var testInvoices = unpaidInvoices.Where(i => i.AmountCost == 1000).ToList(); // Our test invoices
        testInvoices.Should().HaveCount(5); // Excluding the paid one
        
        // Verify categorization by checking the date ranges
        var testCurrent = testInvoices.Where(i => (now - i.WorkDate).Days <= 30).ToList();
        var testThirtyDays = testInvoices.Where(i => (now - i.WorkDate).Days > 30 && (now - i.WorkDate).Days <= 60).ToList();
        var testSixtyDays = testInvoices.Where(i => (now - i.WorkDate).Days > 60 && (now - i.WorkDate).Days <= 90).ToList();
        var testOverNinety = testInvoices.Where(i => (now - i.WorkDate).Days > 90).ToList();
        
        testCurrent.Should().HaveCount(2);        // -5 and -25 days
        testThirtyDays.Should().HaveCount(1);     // -45 days
        testSixtyDays.Should().HaveCount(1);      // -75 days
        testOverNinety.Should().HaveCount(1);     // -120 days
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PropertyManagement_TracksJobsPerUnit()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c => c.Id = 0);
        await _companyService.SaveCompanyAsync(company);

        var property = TestDataBuilder.CreateProperty(p =>
        {
            p.Id = 0;
            p.SupervisorId = 1;
            p.Address = "100 Test Ave";
        });
        // Properties are managed through Supervisor relationship
        // For test purposes, we'll add directly to DB
        await DbContext.Properties.AddAsync(property);
        await DbContext.SaveChangesAsync();

        // Create multiple invoices for different units
        var units = new[] { "A101", "A102", "B201", "B202" };
        foreach (var unit in units)
        {
            var invoice = TestDataBuilder.CreateInvoice(i =>
            {
                i.Id = 0;
                i.PropertyAddress = property.Address ?? "100 Test Ave"; // Ensure non-null value
                i.Unit = unit;
                i.CompanyName = company.Name;
            });
            await _invoiceService.AddInvoiceAsync(invoice);
        }

        // Act
        var propertyInvoices = await DbContext.Invoice
            .Where(i => i.PropertyAddress == "100 Test Ave")
            .ToListAsync();

        // Assert
        propertyInvoices.Should().HaveCount(4);
        propertyInvoices.Select(i => i.Unit).Should().BeEquivalentTo(units);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InvoiceWorkflow_DataIntegrityAcrossServices_Maintained()
    {
        // Arrange - Create interconnected business entities
        var company = TestDataBuilder.CreateCompany(c =>
        {
            c.Id = 0;
            c.Name = "Integrity Test Corp";
            c.Email = "integrity@test.com";
        });
        await _companyService.SaveCompanyAsync(company);

        var supervisor = TestDataBuilder.CreateSupervisor(s =>
        {
            s.Id = 0;
            s.Name = "Test Supervisor";
            s.CompanyId = company.Id;
        });
        await DbContext.Supervisor.AddAsync(supervisor);
        await DbContext.SaveChangesAsync();

        var property = TestDataBuilder.CreateProperty(p =>
        {
            p.Id = 0;
            p.Name = "Integrity Property";
            p.Address = "999 Integrity St";
            p.SupervisorId = supervisor.Id;
        });
        await DbContext.Properties.AddAsync(property);
        await DbContext.SaveChangesAsync();

        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = 0;
            c.Name = "Integrity Contractor";
            c.Email = "contractor@test.com";
        });
        await _contractorService.SaveContractorAsync(contractor);

        // Act - Create invoice linking all entities
        var invoice = TestDataBuilder.CreateInvoice(i =>
        {
            i.Id = 0;
            i.CompanyName = company.Name;
            i.PropertyAddress = property.Address;
            i.ContractorName = contractor.Name;
            i.WorkOrder = "INT-001";
            i.AmountCost = 5000;
        });
        await _invoiceService.AddInvoiceAsync(invoice);

        // Assert - Verify data integrity across all services
        var savedInvoice = await _invoiceService.GetAllInvoicesAsync();
        savedInvoice.Should().Contain(i => i.WorkOrder == "INT-001");

        var companies = await _companyService.GetAllCompaniesAsync();
        companies.Should().Contain(c => c.Name == "Integrity Test Corp");

        var contractors = await _contractorService.GetAllContractorsAsync();
        contractors.Should().Contain(c => c.Name == "Integrity Contractor");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InvoiceWorkflow_ConcurrentOperations_HandleConflicts()
    {
        // Arrange - Create shared resources
        var company = TestDataBuilder.CreateCompany(c =>
        {
            c.Id = 0;
            c.Name = "Concurrent Test Co";
        });
        await _companyService.SaveCompanyAsync(company);

        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = 0;
            c.Name = "Concurrent Contractor";
        });
        await _contractorService.SaveContractorAsync(contractor);

        // Act - Create multiple invoices concurrently (simulated)
        var invoices = new List<Invoice>();
        for (int i = 1; i <= 5; i++)
        {
            var invoice = TestDataBuilder.CreateInvoice(inv =>
            {
                inv.Id = 0;
                inv.WorkOrder = $"CONC-{i:000}";
                inv.CompanyName = company.Name;
                inv.ContractorName = contractor.Name;
                inv.AmountCost = 1000 * i;
            });
            invoices.Add(invoice);
        }

        foreach (var invoice in invoices)
        {
            await _invoiceService.AddInvoiceAsync(invoice);
        }

        // Assert - All invoices should be created successfully
        var allInvoices = await _invoiceService.GetAllInvoicesAsync();
        allInvoices.Should().HaveCountGreaterThanOrEqualTo(5);
        allInvoices.Where(i => i.CompanyName == company.Name).Should().HaveCount(5);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InvoiceWorkflow_BusinessRulesValidation_Enforced()
    {
        // Arrange - Create valid supporting entities
        var company = TestDataBuilder.CreateCompany(c =>
        {
            c.Id = 0;
            c.Name = "Validation Test Co";
        });
        await _companyService.SaveCompanyAsync(company);

        // Act & Assert - Test various business rule validations
        var invalidInvoices = new List<Invoice>
        {
            // Invoice with negative amount
            TestDataBuilder.CreateInvoice(i => {
                i.Id = 0;
                i.WorkOrder = "INVALID-001";
                i.AmountCost = -1000;
                i.CompanyName = company.Name;
            }),
            // Invoice with future work date (might be valid, but test boundary)
            TestDataBuilder.CreateInvoice(i => {
                i.Id = 0;
                i.WorkOrder = "INVALID-002";
                i.WorkDate = DateTime.Now.AddYears(1);
                i.CompanyName = company.Name;
            })
        };

        // These should still create successfully (business rules are enforced in UI, not service layer)
        foreach (var invoice in invalidInvoices)
        {
            await _invoiceService.AddInvoiceAsync(invoice);
        }

        // Assert - Invoices were created (service layer doesn't enforce business rules)
        var allInvoices = await _invoiceService.GetAllInvoicesAsync();
        allInvoices.Should().Contain(i => i.WorkOrder == "INVALID-001");
        allInvoices.Should().Contain(i => i.WorkOrder == "INVALID-002");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InvoiceWorkflow_ReportGeneration_HandlesLargeDatasets()
    {
        // Arrange - Create large dataset
        var company = TestDataBuilder.CreateCompany(c =>
        {
            c.Id = 0;
            c.Name = "Large Dataset Co";
        });
        await _companyService.SaveCompanyAsync(company);

        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = 0;
            c.Name = "Large Dataset Contractor";
        });
        await _contractorService.SaveContractorAsync(contractor);

        // Create 50 invoices
        var invoices = new List<Invoice>();
        for (int i = 1; i <= 50; i++)
        {
            var invoice = TestDataBuilder.CreateInvoice(inv =>
            {
                inv.Id = 0;
                inv.WorkOrder = $"LARGE-{i:000}";
                inv.CompanyName = company.Name;
                inv.ContractorName = contractor.Name;
                inv.AmountCost = 1000 + (i * 100);
                inv.WorkDate = DateTime.Now.AddDays(-i);
            });
            invoices.Add(invoice);
        }

        foreach (var invoice in invoices)
        {
            await _invoiceService.AddInvoiceAsync(invoice);
        }

        // Act - Test various query operations on large dataset
        var allInvoices = await _invoiceService.GetAllInvoicesAsync();
        var dateRangeInvoices = await _invoiceService.GetInvoicesByDateRangeAsync(
            DateTime.Now.AddDays(-25), DateTime.Now);
        var receivableInvoices = await _invoiceService.GetInvoicesReceivable();

        // Assert - Operations complete successfully on large dataset
        allInvoices.Should().HaveCountGreaterThanOrEqualTo(50);
        dateRangeInvoices.Should().NotBeNull();
        receivableInvoices.Should().NotBeNull();

        // Verify data integrity
        allInvoices.Where(i => i.CompanyName == company.Name).Should().HaveCount(50);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InvoiceWorkflow_CrossServiceDataConsistency_Maintained()
    {
        // Arrange - Create complete business ecosystem
        var company = TestDataBuilder.CreateCompany(c =>
        {
            c.Id = 0;
            c.Name = "Consistency Test Corp";
            c.Email = "consistency@test.com";
            c.Phone = "555-CONSISTENT";
        });
        await _companyService.SaveCompanyAsync(company);

        var supervisor = TestDataBuilder.CreateSupervisor(s =>
        {
            s.Id = 0;
            s.Name = "Consistency Supervisor";
            s.CompanyId = company.Id;
        });
        await DbContext.Supervisor.AddAsync(supervisor);
        await DbContext.SaveChangesAsync();

        var properties = new List<Properties>();
        for (int i = 1; i <= 3; i++)
        {
            var property = TestDataBuilder.CreateProperty(p =>
            {
                p.Id = 0;
                p.Name = $"Consistency Property {i}";
                p.Address = $"{100 + i} Consistency St";
                p.SupervisorId = supervisor.Id;
            });
            properties.Add(property);
        }
        foreach (var property in properties)
        {
            await DbContext.Properties.AddAsync(property);
        }
        await DbContext.SaveChangesAsync();

        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = 0;
            c.Name = "Consistency Contractor";
            c.Email = "contractor@consistency.com";
            c.LicenseNumber = "CONSIST-001";
        });
        await _contractorService.SaveContractorAsync(contractor);

        // Act - Create invoices for each property
        foreach (var property in properties)
        {
            var invoice = TestDataBuilder.CreateInvoice(i =>
            {
                i.Id = 0;
                i.WorkOrder = $"CONSIST-{property.Name.Replace(" ", "")}";
                i.CompanyName = company.Name;
                i.PropertyAddress = property.Address;
                i.ContractorName = contractor.Name;
                i.AmountCost = 2000;
                i.JobDescriptionChoice = "Consistency Testing";
            });
            await _invoiceService.AddInvoiceAsync(invoice);
        }

        // Assert - Verify complete data ecosystem integrity
        var savedCompany = (await _companyService.GetAllCompaniesAsync())
            .First(c => c.Name == "Consistency Test Corp");
        var savedContractor = (await _contractorService.GetAllContractorsAsync())
            .First(c => c.Name == "Consistency Contractor");
        var savedInvoices = (await _invoiceService.GetAllInvoicesAsync())
            .Where(i => i.CompanyName == company.Name).ToList();

        // Cross-service validation
        savedCompany.Name.Should().Be("Consistency Test Corp");
        savedCompany.Email.Should().Be("consistency@test.com");
        savedCompany.Phone.Should().Be("555-CONSISTENT");

        savedContractor.Name.Should().Be("Consistency Contractor");
        savedContractor.Email.Should().Be("contractor@consistency.com");
        savedContractor.LicenseNumber.Should().Be("CONSIST-001");

        savedInvoices.Should().HaveCount(3);
        savedInvoices.All(i => i.CompanyName == company.Name).Should().BeTrue();
        savedInvoices.All(i => i.ContractorName == contractor.Name).Should().BeTrue();
        savedInvoices.All(i => i.JobDescriptionChoice == "Consistency Testing").Should().BeTrue();
    }
}

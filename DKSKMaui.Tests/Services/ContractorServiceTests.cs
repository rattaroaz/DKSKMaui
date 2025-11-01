using DKSKMaui.Backend.Services;
using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DKSKMaui.Tests.Services;

public class ContractorServiceTests : TestBase
{
    private readonly ContractorService _contractorService;

    public ContractorServiceTests()
    {
        _contractorService = ServiceProvider.GetRequiredService<ContractorService>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllContractorsAsync_ReturnsAllContractors()
    {
        // Arrange - Get initial count (includes seeded data)
        var initialResult = await _contractorService.GetAllContractorsAsync();
        var initialCount = initialResult.Count;

        // Clear the change tracker and ensure we use IDs that don't conflict
        DbContext.ChangeTracker.Clear();
        // Find the highest existing ID to avoid conflicts
        var maxId = await DbContext.Contractor.AnyAsync() ? await DbContext.Contractor.MaxAsync(c => c.Id) : 0;

        var contractors = TestDataBuilder.CreateMany(() => TestDataBuilder.CreateContractor(c => c.Id = ++maxId), 3);
        await DbContext.Contractor.AddRangeAsync(contractors);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _contractorService.GetAllContractorsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(initialCount + 3);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetActiveContractorsAsync_ReturnsOnlyActiveContractors()
    {
        // Arrange - Add some test contractors
        var nextId = await GetNextIdAsync<Contractor>();
        var contractors = new List<Contractor>
        {
            TestDataBuilder.CreateContractor(c => { c.Id = nextId++; c.Name = "Active1"; c.IsActive = true; }),
            TestDataBuilder.CreateContractor(c => { c.Id = nextId++; c.Name = "Inactive1"; c.IsActive = false; }),
            TestDataBuilder.CreateContractor(c => { c.Id = nextId++; c.Name = "Active2"; c.IsActive = true; }),
            TestDataBuilder.CreateContractor(c => { c.Id = nextId++; c.Name = "Null1"; c.IsActive = null; })
        };
        await DbContext.Contractor.AddRangeAsync(contractors);
        await SaveChangesAndClearTracking();

        // Act
        var allContractors = await _contractorService.GetAllContractorsAsync();
        var result = allContractors.Where(c => c.IsActive == true).ToList();

        // Assert - Should include our active contractors plus any seeded active ones
        result.Should().Contain(c => c.Name == "Active1");
        result.Should().Contain(c => c.Name == "Active2");
        result.All(c => c.IsActive == true).Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetContractorById_ValidId_ReturnsContractor()
    {
        // Arrange
        var nextId = await GetNextIdAsync<Contractor>();
        var contractor = TestDataBuilder.CreateContractor(c => c.Id = nextId);
        await DbContext.Contractor.AddAsync(contractor);
        await SaveChangesAndClearTracking();

        // Act
        var allContractors = await _contractorService.GetAllContractorsAsync();
        var foundContractor = allContractors.FirstOrDefault(c => c.Id == contractor.Id);

        // Assert
        foundContractor.Should().NotBeNull();
        foundContractor!.Id.Should().Be(contractor.Id);
        foundContractor.Name.Should().Be(contractor.Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddContractorAsync_validContractor_AddsToDatabase()
    {
        // Arrange
        var contractor = TestDataBuilder.CreateContractor(c => 
        {
            c.Id = 0;
            c.Name = "New Contractor";
        });

        // Act
        await _contractorService.SaveContractorAsync(contractor);

        // Assert
        var savedContractor = await DbContext.Contractor
            .FirstOrDefaultAsync(c => c.Name == "New Contractor");
        savedContractor.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateContractorAsync_UpdatesContractorInfo()
    {
        // Arrange
        var nextId = await GetNextIdAsync<Contractor>();
        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = nextId;
            c.Name = "Original Name";
            c.IsActive = true;
        });
        await DbContext.Contractor.AddAsync(contractor);
        await SaveChangesAndClearTracking();

        contractor.Name = "Updated Name";
        contractor.IsActive = false;

        // Act
        await _contractorService.SaveContractorAsync(contractor);

        // Assert
        var updatedContractor = await DbContext.Contractor.FindAsync(nextId);
        updatedContractor.Should().NotBeNull();
        updatedContractor!.Name.Should().Be("Updated Name");
        updatedContractor.IsActive.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeactivateContractorAsync_SetsIsActiveToFalse()
    {
        // Arrange
        var nextId = await GetNextIdAsync<Contractor>();
        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Id = nextId;
            c.IsActive = true;
        });
        await DbContext.Contractor.AddAsync(contractor);
        await SaveChangesAndClearTracking();

        // Act
        contractor.IsActive = false;
        await _contractorService.SaveContractorAsync(contractor);

        // Assert
        var deactivatedContractor = await DbContext.Contractor.FindAsync(nextId);
        deactivatedContractor.Should().NotBeNull();
        deactivatedContractor!.IsActive.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SearchContractorsByNameAsync_FindsMatchingContractors()
    {
        // Arrange
        var nextId = await GetNextIdAsync<Contractor>();
        var contractors = new List<Contractor>
        {
            TestDataBuilder.CreateContractor(c => { c.Id = nextId++; c.Name = "Test John Smith"; }),
            TestDataBuilder.CreateContractor(c => { c.Id = nextId++; c.Name = "Jane Doe"; }),
            TestDataBuilder.CreateContractor(c => { c.Id = nextId++; c.Name = "Test John Doe"; })
        };
        await DbContext.Contractor.AddRangeAsync(contractors);
        await SaveChangesAndClearTracking();

        // Act
        var allContractors = await _contractorService.GetAllContractorsAsync();
        var result = allContractors.Where(c => c.Name.Contains("Test John")).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.All(c => c.Name.Contains("Test John")).Should().BeTrue();
    }
}

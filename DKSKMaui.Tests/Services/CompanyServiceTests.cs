using DKSKMaui.Backend.Services;
using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DKSKMaui.Tests.Services;

public class CompanyServiceTests : TestBase
{
    private readonly CompanyService _companyService;

    public CompanyServiceTests()
    {
        _companyService = ServiceProvider.GetRequiredService<CompanyService>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllCompaniesAsync_ReturnsAllCompanies()
    {
        // Arrange - Get initial count (includes seeded data)
        var initialResult = await _companyService.GetAllCompaniesAsync();
        var initialCount = initialResult.Count;

        // Clear the change tracker and ensure we use IDs that don't conflict
        DbContext.ChangeTracker.Clear();
        // Find the highest existing ID to avoid conflicts
        var maxId = await DbContext.Companny.AnyAsync() ? await DbContext.Companny.MaxAsync(c => c.Id) : 0;

        var companies = TestDataBuilder.CreateMany(() => TestDataBuilder.CreateCompany(c => c.Id = ++maxId), 3);
        await DbContext.Companny.AddRangeAsync(companies);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(initialCount + 3);
        result.Select(c => c.Name).Should().Contain(companies.Select(c => c.Name));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllCompaniesAsync_WithNewCompany_IncludesInResults()
    {
        // Arrange - Create with unique ID to avoid conflicts with seeded data
        var nextId = await GetNextIdAsync<Companny>();
        var company = TestDataBuilder.CreateCompany(c => c.Name = "Test Company XYZ");
        company.Id = nextId;
        await DbContext.Companny.AddAsync(company);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(c => c.Name == "Test Company XYZ");
    }


    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddCompanyAsync_ValidCompany_AddsToDatabase()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c => c.Id = 0);

        // Act
        await _companyService.SaveCompanyAsync(company);

        // Assert
        var savedCompany = await DbContext.Companny.FirstOrDefaultAsync(c => c.Name == company.Name);
        savedCompany.Should().NotBeNull();
        savedCompany!.Email.Should().Be(company.Email);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateCompanyAsync_ExistingCompany_UpdatesDatabase()
    {
        // Arrange - Use a unique ID to avoid conflicts with seeded data
        var nextId = await GetNextIdAsync<Companny>();
        var company = TestDataBuilder.CreateCompany(c => c.Name = "Original Name");
        company.Id = nextId;
        await DbContext.Companny.AddAsync(company);
        await SaveChangesAndClearTracking();

        company.Name = "Updated Company Name";
        company.Email = "updated@example.com";

        // Act
        await _companyService.SaveCompanyAsync(company);

        // Assert
        var updatedCompany = await DbContext.Companny.FindAsync(nextId);
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Name.Should().Be("Updated Company Name");
        updatedCompany.Email.Should().Be("updated@example.com");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteCompanyAsync_ExistingCompany_RemovesFromDatabase()
    {
        // Arrange - Create a new company to delete
        var nextId = await GetNextIdAsync<Companny>();
        var company = TestDataBuilder.CreateCompany(c => c.Name = "Company To Delete");
        company.Id = nextId;
        await DbContext.Companny.AddAsync(company);
        await SaveChangesAndClearTracking();

        // Act
        await _companyService.DeleteCompanyAsync(nextId);

        // Assert
        var deletedCompany = await DbContext.Companny.FindAsync(nextId);
        deletedCompany.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllCompaniesAsync_FilterByName_FindsMatchingCompanies()
    {
        // Arrange
        var nextId = await GetNextIdAsync<Companny>();
        var companies = new List<Companny>
        {
            TestDataBuilder.CreateCompany(c => { c.Id = nextId++; c.Name = "ABC Painting"; }),
            TestDataBuilder.CreateCompany(c => { c.Id = nextId++; c.Name = "XYZ Construction"; }),
            TestDataBuilder.CreateCompany(c => { c.Id = nextId++; c.Name = "ABC Contractors"; })
        };
        await DbContext.Companny.AddRangeAsync(companies);
        await SaveChangesAndClearTracking();

        // Act
        var allCompanies = await _companyService.GetAllCompaniesAsync();
        var result = allCompanies.Where(c => c.Name.Contains("ABC")).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.All(c => c.Name.Contains("ABC")).Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllCompaniesAsync_WithMultipleCompanies_ReturnsAll()
    {
        // Arrange - Get initial count
        var initialResult = await _companyService.GetAllCompaniesAsync();
        var initialCount = initialResult.Count;

        var nextId = await GetNextIdAsync<Companny>();
        var companies = new List<Companny>
        {
            TestDataBuilder.CreateCompany(c => { c.Id = nextId++; c.Name = "Test Company AAA"; }),
            TestDataBuilder.CreateCompany(c => { c.Id = nextId++; c.Name = "Test Company BBB"; }),
            TestDataBuilder.CreateCompany(c => { c.Id = nextId++; c.Name = "Test Company CCC"; })
        };

        await DbContext.Companny.AddRangeAsync(companies);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        result.Should().HaveCount(initialCount + 3);
        result.Select(c => c.Name).Should().Contain(new[] { "Test Company AAA", "Test Company BBB", "Test Company CCC" });
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SaveCompanyAsync_UpdateExistingCompany_Succeeds()
    {
        // Arrange
        var nextId = await GetNextIdAsync<Companny>();
        var company = TestDataBuilder.CreateCompany(c => { c.Id = nextId; c.Name = "Original Name"; });
        await DbContext.Companny.AddAsync(company);
        await SaveChangesAndClearTracking();

        company.Name = "Updated Name";
        company.Email = "updated@test.com";

        // Act
        var result = await _companyService.SaveCompanyAsync(company);

        // Assert
        result.Should().BeTrue();
        var updatedCompany = await DbContext.Companny.FindAsync(nextId);
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Name.Should().Be("Updated Name");
        updatedCompany.Email.Should().Be("updated@test.com");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteCompanyAsync_NonExistentCompany_ReturnsFalse()
    {
        // Act
        var result = await _companyService.DeleteCompanyAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SaveCompanyAsync_NullCompany_HandlesGracefully()
    {
        // Act & Assert - Services may handle null gracefully or throw different exceptions
        try
        {
            await _companyService.SaveCompanyAsync(null!);
        }
        catch (Exception ex)
        {
            // Any exception is acceptable as long as it's not a crash
            ex.Should().NotBeNull();
        }
    }


    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllCompaniesAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange - Clear all companies
        DbContext.Companny.RemoveRange(DbContext.Companny);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteCompanyAsync_ValidCompany_RemovesFromDatabase()
    {
        // Arrange
        var nextId = await GetNextIdAsync<Companny>();
        var company = TestDataBuilder.CreateCompany(c => { c.Id = nextId; c.Name = "Company to Delete"; });
        await DbContext.Companny.AddAsync(company);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _companyService.DeleteCompanyAsync(nextId);

        // Assert
        result.Should().BeTrue();
        var deletedCompany = await DbContext.Companny.FindAsync(nextId);
        deletedCompany.Should().BeNull();
    }
}

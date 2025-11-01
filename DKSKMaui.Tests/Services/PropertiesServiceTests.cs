using DKSKMaui.Backend.Services;
using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DKSKMaui.Tests.Services;

public class PropertiesServiceTests : TestBase
{
    private readonly PropertiesService _service;
    private readonly Mock<ILogger<PropertiesService>> _mockLogger;

    public PropertiesServiceTests()
    {
        _mockLogger = new Mock<ILogger<PropertiesService>>();
        _service = new PropertiesService(DbContext, _mockLogger.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllPropertiesAsync_WhenNoProperties_ReturnsEmptyList()
    {
        // Act
        var result = await _service.GetAllPropertiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllPropertiesAsync_WhenPropertiesExist_ReturnsAllProperties()
    {
        // Arrange
        var properties = new List<Properties>
        {
            TestDataBuilder.CreateProperty(p => { p.Id = 1001; p.Name = "Property 1"; p.Address = "123 Main St"; }),
            TestDataBuilder.CreateProperty(p => { p.Id = 1002; p.Name = "Property 2"; p.Address = "456 Oak Ave"; }),
            TestDataBuilder.CreateProperty(p => { p.Id = 1003; p.Name = "Property 3"; p.Address = "789 Pine Rd"; })
        };
        await DbContext.Properties.AddRangeAsync(properties);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.GetAllPropertiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Property 1");
        result.Should().Contain(p => p.Name == "Property 2");
        result.Should().Contain(p => p.Name == "Property 3");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllPropertiesAsync_WhenExceptionOccurs_LogsErrorAndReturnsEmptyList()
    {
        // Arrange - Force an exception by disposing the context
        DbContext.Dispose();

        // Act
        var result = await _service.GetAllPropertiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error while getting properties")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPropertiesByCompanyIdAsync_WhenNoMatchingProperties_ReturnsEmptyList()
    {
        // Arrange - Add properties with different company
        var company1 = TestDataBuilder.CreateCompany(c => c.Name = "Company 1");
        var supervisor1 = TestDataBuilder.CreateSupervisor(s => { s.Company = company1; s.Name = "Supervisor 1"; });
        
        await DbContext.Companny.AddAsync(company1);
        await DbContext.Supervisor.AddAsync(supervisor1);
        
        var property = TestDataBuilder.CreateProperty(p => 
        { 
            p.SupervisorId = supervisor1.Id;
            p.Supervisor = supervisor1;
        });
        await DbContext.Properties.AddAsync(property);
        await SaveChangesAndClearTracking();

        // Act - Search for different company ID
        var result = await _service.GetPropertiesByCompanyIdAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPropertiesByCompanyIdAsync_WhenMatchingPropertiesExist_ReturnsFilteredProperties()
    {
        // Arrange - Create companies and supervisors
        var company1 = TestDataBuilder.CreateCompany(c => { c.Name = "Company 1"; });
        var company2 = TestDataBuilder.CreateCompany(c => { c.Name = "Company 2"; });
        
        await DbContext.Companny.AddRangeAsync(company1, company2);
        
        var supervisor1 = TestDataBuilder.CreateSupervisor(s => 
        { 
            s.CompanyId = company1.Id;
            s.Company = company1;
            s.Name = "Supervisor 1";
        });
        
        var supervisor2 = TestDataBuilder.CreateSupervisor(s => 
        { 
            s.CompanyId = company2.Id;
            s.Company = company2;
            s.Name = "Supervisor 2";
        });
        
        await DbContext.Supervisor.AddRangeAsync(supervisor1, supervisor2);
        
        // Add properties for different supervisors/companies
        var properties = new List<Properties>
        {
            TestDataBuilder.CreateProperty(p => 
            { 
                p.Name = "Property A";
                p.SupervisorId = supervisor1.Id;
                p.Supervisor = supervisor1;
            }),
            TestDataBuilder.CreateProperty(p => 
            { 
                p.Name = "Property B";
                p.SupervisorId = supervisor1.Id;
                p.Supervisor = supervisor1;
            }),
            TestDataBuilder.CreateProperty(p => 
            { 
                p.Name = "Property C";
                p.SupervisorId = supervisor2.Id;
                p.Supervisor = supervisor2;
            })
        };
        
        await DbContext.Properties.AddRangeAsync(properties);
        await SaveChangesAndClearTracking();

        // Act - Get properties for company 1
        var result = await _service.GetPropertiesByCompanyIdAsync(company1.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Property A");
        result.Should().Contain(p => p.Name == "Property B");
        result.Should().NotContain(p => p.Name == "Property C");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPropertiesByCompanyIdAsync_IncludesSupervisorData()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany();
        var supervisor = TestDataBuilder.CreateSupervisor(s => 
        { 
            s.CompanyId = company.Id;
            s.Company = company;
            s.Name = "John Supervisor";
            s.Email = "john@company.com";
        });
        
        await DbContext.Companny.AddAsync(company);
        await DbContext.Supervisor.AddAsync(supervisor);
        
        var property = TestDataBuilder.CreateProperty(p => 
        { 
            p.Name = "Test Property";
            p.SupervisorId = supervisor.Id;
            p.Supervisor = supervisor;
        });
        await DbContext.Properties.AddAsync(property);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.GetPropertiesByCompanyIdAsync(company.Id);

        // Assert
        result.Should().HaveCount(1);
        var retrievedProperty = result.First();
        retrievedProperty.Supervisor.Should().NotBeNull();
        retrievedProperty.Supervisor.Name.Should().Be("John Supervisor");
        retrievedProperty.Supervisor.Email.Should().Be("john@company.com");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPropertiesByCompanyIdAsync_WhenExceptionOccurs_LogsErrorAndReturnsEmptyList()
    {
        // Arrange - Force an exception by disposing the context
        DbContext.Dispose();

        // Act
        var result = await _service.GetPropertiesByCompanyIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains("Error while getting properties by company ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public async Task GetPropertiesByCompanyIdAsync_WithVariousIds_HandlesCorrectly(int companyId)
    {
        // Act
        var result = await _service.GetPropertiesByCompanyIdAsync(companyId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

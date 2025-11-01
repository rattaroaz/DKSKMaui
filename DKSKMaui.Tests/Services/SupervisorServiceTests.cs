using DKSKMaui.Backend.Services;
using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DKSKMaui.Tests.Services;

[Collection("Database")]
public class SupervisorServiceTests : TestBase
{
    private readonly SupervisorService _service;
    private readonly Mock<ILogger<SupervisorService>> _mockLogger;

    public SupervisorServiceTests()
    {
        _mockLogger = new Mock<ILogger<SupervisorService>>();
        _service = new SupervisorService(DbContext, _mockLogger.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllSupervisorsAsync_WhenNoSupervisors_ReturnsEmptyList()
    {
        // Act
        var result = await _service.GetAllSupervisorsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllSupervisorsAsync_WhenSupervisorsExist_ReturnsAllWithRelatedData()
    {
        // Arrange
        var company1 = TestDataBuilder.CreateCompany(c => { c.Id = 2001; c.Name = "Company 1"; });
        var company2 = TestDataBuilder.CreateCompany(c => { c.Id = 2002; c.Name = "Company 2"; });
        
        await DbContext.Companny.AddRangeAsync(company1, company2);
        
        var supervisors = new List<Supervisor>
        {
            TestDataBuilder.CreateSupervisor(s => 
            { 
                s.Id = 3001;
                s.Name = "Supervisor 1";
                s.CompanyId = company1.Id;
            }),
            TestDataBuilder.CreateSupervisor(s => 
            { 
                s.Id = 3002;
                s.Name = "Supervisor 2";
                s.CompanyId = company2.Id;
            })
        };
        
        await DbContext.Supervisor.AddRangeAsync(supervisors);
        
        // Add properties for supervisor 1
        var properties = new List<Properties>
        {
            TestDataBuilder.CreateProperty(p => 
            { 
                p.Id = 4001;
                p.Name = "Property 1";
                p.SupervisorId = supervisors[0].Id;
            }),
            TestDataBuilder.CreateProperty(p => 
            { 
                p.Id = 4002;
                p.Name = "Property 2";
                p.SupervisorId = supervisors[0].Id;
            })
        };
        
        await DbContext.Properties.AddRangeAsync(properties);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.GetAllSupervisorsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        
        var supervisor1 = result.First(s => s.Name == "Supervisor 1");
        supervisor1.Company.Should().NotBeNull();
        supervisor1.Company!.Name.Should().Be("Company 1");
        supervisor1.Properties.Should().NotBeNull();
        supervisor1.Properties.Should().HaveCount(2);
        
        var supervisor2 = result.First(s => s.Name == "Supervisor 2");
        supervisor2.Company.Should().NotBeNull();
        supervisor2.Company!.Name.Should().Be("Company 2");
        supervisor2.Properties.Should().NotBeNull();
        supervisor2.Properties.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllSupervisorsAsync_IncludesCompanyData()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c => 
        { 
            c.Name = "Test Company";
            c.Email = "test@company.com";
            c.Phone = "123-456-7890";
        });
        
        await DbContext.Companny.AddAsync(company);
        
        var supervisor = TestDataBuilder.CreateSupervisor(s => 
        { 
            s.Name = "Test Supervisor";
            s.CompanyId = company.Id;
        });
        
        await DbContext.Supervisor.AddAsync(supervisor);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.GetAllSupervisorsAsync();

        // Assert
        result.Should().HaveCount(1);
        var retrievedSupervisor = result.First();
        retrievedSupervisor.Company.Should().NotBeNull();
        retrievedSupervisor.Company!.Name.Should().Be("Test Company");
        retrievedSupervisor.Company.Email.Should().Be("test@company.com");
        retrievedSupervisor.Company.Phone.Should().Be("123-456-7890");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllSupervisorsAsync_IncludesPropertiesData()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c => c.Name = "Test Company");
        await DbContext.Companny.AddAsync(company);
        
        var supervisor = TestDataBuilder.CreateSupervisor(s => 
        { 
            s.Name = "Supervisor with Properties";
            s.CompanyId = company.Id;
        });
        await DbContext.Supervisor.AddAsync(supervisor);
        
        var properties = new List<Properties>
        {
            TestDataBuilder.CreateProperty(p => 
            { 
                p.Name = "Property A";
                p.Address = "123 Main St";
                p.SupervisorId = supervisor.Id;
            }),
            TestDataBuilder.CreateProperty(p => 
            { 
                p.Name = "Property B";
                p.Address = "456 Oak Ave";
                p.SupervisorId = supervisor.Id;
            }),
            TestDataBuilder.CreateProperty(p => 
            { 
                p.Name = "Property C";
                p.Address = "789 Pine Rd";
                p.SupervisorId = supervisor.Id;
            })
        };
        
        await DbContext.Properties.AddRangeAsync(properties);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.GetAllSupervisorsAsync();

        // Assert
        result.Should().HaveCount(1);
        var retrievedSupervisor = result.First();
        retrievedSupervisor.Properties.Should().NotBeNull();
        retrievedSupervisor.Properties.Should().HaveCount(3);
        retrievedSupervisor.Properties.Should().Contain(p => p.Name == "Property A");
        retrievedSupervisor.Properties.Should().Contain(p => p.Name == "Property B");
        retrievedSupervisor.Properties.Should().Contain(p => p.Name == "Property C");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllSupervisorsAsync_WhenSupervisorHasNoCompany_StillReturns()
    {
        // Arrange - Create a company first
        var company = TestDataBuilder.CreateCompany(c => c.Name = "Test Company");
        await DbContext.Companny.AddAsync(company);
        
        // Supervisor with valid company
        var supervisor = TestDataBuilder.CreateSupervisor(s => 
        { 
            s.Name = "Test Supervisor";
            s.CompanyId = company.Id;
        });
        
        await DbContext.Supervisor.AddAsync(supervisor);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.GetAllSupervisorsAsync();

        // Assert
        result.Should().HaveCount(1);
        var retrievedSupervisor = result.First();
        retrievedSupervisor.Name.Should().Be("Test Supervisor");
        retrievedSupervisor.Company.Should().NotBeNull();
        retrievedSupervisor.Company!.Name.Should().Be("Test Company");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllSupervisorsAsync_WhenExceptionOccurs_LogsErrorAndReturnsEmptyList()
    {
        // Arrange - Force an exception by disposing the context
        DbContext.Dispose();

        // Act
        var result = await _service.GetAllSupervisorsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains("Error while getting supervisors")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}

using DKSKMaui.Backend.Services;
using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DKSKMaui.Tests.Services;

public class JobDescriptionServiceTests : TestBase
{
    private readonly JobDescriptionService _service;
    private readonly Mock<ILogger<JobDescriptionService>> _mockLogger;

    public JobDescriptionServiceTests()
    {
        _mockLogger = new Mock<ILogger<JobDescriptionService>>();
        _service = new JobDescriptionService(DbContext, _mockLogger.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllJobsAsync_WhenNoJobs_ReturnsEmptyList()
    {
        // Act
        var result = await _service.GetAllJobsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllJobsAsync_WhenJobsExist_ReturnsAllJobs()
    {
        // Arrange
        var jobs = new List<JobDiscription>
        {
            TestDataBuilder.CreateJobDescription(j => { j.description = "Interior Painting"; j.price = 500; }),
            TestDataBuilder.CreateJobDescription(j => { j.description = "Exterior Painting"; j.price = 750; }),
            TestDataBuilder.CreateJobDescription(j => { j.description = "Kitchen Cabinets"; j.price = 1200; })
        };
        await DbContext.JobDiscription.AddRangeAsync(jobs);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.GetAllJobsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(j => j.description == "Interior Painting");
        result.Should().Contain(j => j.description == "Exterior Painting");
        result.Should().Contain(j => j.description == "Kitchen Cabinets");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllJobsAsync_WhenExceptionOccurs_LogsErrorAndReturnsEmptyList()
    {
        // Arrange - Force an exception by disposing the context
        DbContext.Dispose();

        // Act
        var result = await _service.GetAllJobsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error while getting job descriptions")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReplaceAll_WhenNoExistingJobs_AddsNewJobs()
    {
        // Arrange
        var newJobs = new List<JobDiscription>
        {
            TestDataBuilder.CreateJobDescription(j => { j.description = "New Job 1"; j.price = 100; }),
            TestDataBuilder.CreateJobDescription(j => { j.description = "New Job 2"; j.price = 200; })
        };

        // Act
        var result = await _service.ReplaceAll(newJobs);

        // Assert
        result.Should().BeTrue();
        var savedJobs = await DbContext.JobDiscription.ToListAsync();
        savedJobs.Should().HaveCount(2);
        savedJobs.Should().Contain(j => j.description == "New Job 1");
        savedJobs.Should().Contain(j => j.description == "New Job 2");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReplaceAll_WhenExistingJobsPresent_RemovesOldAndAddsNew()
    {
        // Arrange - Add existing jobs
        var existingJobs = new List<JobDiscription>
        {
            TestDataBuilder.CreateJobDescription(j => { j.description = "Old Job 1"; j.price = 300; }),
            TestDataBuilder.CreateJobDescription(j => { j.description = "Old Job 2"; j.price = 400; }),
            TestDataBuilder.CreateJobDescription(j => { j.description = "Old Job 3"; j.price = 500; })
        };
        await DbContext.JobDiscription.AddRangeAsync(existingJobs);
        await SaveChangesAndClearTracking();

        var newJobs = new List<JobDiscription>
        {
            TestDataBuilder.CreateJobDescription(j => { j.description = "New Job A"; j.price = 600; }),
            TestDataBuilder.CreateJobDescription(j => { j.description = "New Job B"; j.price = 700; })
        };

        // Act
        var result = await _service.ReplaceAll(newJobs);

        // Assert
        result.Should().BeTrue();
        var savedJobs = await DbContext.JobDiscription.ToListAsync();
        savedJobs.Should().HaveCount(2);
        savedJobs.Should().NotContain(j => j.description.StartsWith("Old"));
        savedJobs.Should().Contain(j => j.description == "New Job A");
        savedJobs.Should().Contain(j => j.description == "New Job B");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReplaceAll_WithEmptyList_RemovesAllExistingJobs()
    {
        // Arrange - Add existing jobs
        var existingJobs = new List<JobDiscription>
        {
            TestDataBuilder.CreateJobDescription(j => { j.description = "Job 1"; }),
            TestDataBuilder.CreateJobDescription(j => { j.description = "Job 2"; })
        };
        await DbContext.JobDiscription.AddRangeAsync(existingJobs);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.ReplaceAll(new List<JobDiscription>());

        // Assert
        result.Should().BeTrue();
        var savedJobs = await DbContext.JobDiscription.ToListAsync();
        savedJobs.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReplaceAll_WhenExceptionOccurs_LogsErrorAndReturnsFalse()
    {
        // Arrange
        var newJobs = new List<JobDiscription>
        {
            TestDataBuilder.CreateJobDescription()
        };
        
        // Force an exception by disposing the context
        DbContext.Dispose();

        // Act
        var result = await _service.ReplaceAll(newJobs);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains("Error while replacing job descriptions")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ReplaceAll_WithNullList_LogsErrorAndReturnsFalse()
    {
        // Act
        var result = await _service.ReplaceAll(null!);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains("Error while replacing job descriptions")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(999999)]
    public async Task ReplaceAll_WithVariousPrices_SavesCorrectly(int price)
    {
        // Arrange
        var jobs = new List<JobDiscription>
        {
            TestDataBuilder.CreateJobDescription(j => 
            { 
                j.description = $"Job with price {price}"; 
                j.price = price; 
            })
        };

        // Act
        var result = await _service.ReplaceAll(jobs);

        // Assert
        result.Should().BeTrue();
        var savedJobs = await DbContext.JobDiscription.ToListAsync();
        savedJobs.Should().HaveCount(1);
        savedJobs.First().price.Should().Be(price);
    }
}

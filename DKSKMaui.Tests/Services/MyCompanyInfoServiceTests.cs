using DKSKMaui.Backend.Services;
using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DKSKMaui.Tests.Services;

public class MyCompanyInfoServiceTests : TestBase
{
    private readonly MyCompanyInfoService _service;
    private readonly Mock<ILogger<MyCompanyInfoService>> _mockLogger;

    public MyCompanyInfoServiceTests()
    {
        _mockLogger = new Mock<ILogger<MyCompanyInfoService>>();
        _service = new MyCompanyInfoService(DbContext, _mockLogger.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInfoAsync_WhenNoData_ReturnsNewInstance()
    {
        // Act
        var result = await _service.GetInfoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(0);
        result.Name.Should().BeEmpty();
        result.Email.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetInfoAsync_WhenDataExists_ReturnsExistingInfo()
    {
        // Arrange
        var existingInfo = TestDataBuilder.CreateMyCompanyInfo(c =>
        {
            c.Name = "Test Company";
            c.Email = "test@company.com";
            c.Phone = "123-456-7890";
        });
        await DbContext.MyCompanyInfo.AddAsync(existingInfo);
        await SaveChangesAndClearTracking();

        // Act
        var result = await _service.GetInfoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Company");
        result.Email.Should().Be("test@company.com");
        result.Phone.Should().Be("123-456-7890");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateInfoAsync_WhenNoExistingData_AddsNewInfo()
    {
        // Arrange
        var newInfo = TestDataBuilder.CreateMyCompanyInfo(c =>
        {
            c.Name = "New Company";
            c.Email = "new@company.com";
        });

        // Act
        await _service.UpdateInfoAsync(newInfo);

        // Assert
        var saved = await DbContext.MyCompanyInfo.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("New Company");
        saved.Email.Should().Be("new@company.com");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateInfoAsync_WhenDataExists_UpdatesExistingInfo()
    {
        // Arrange
        var existingInfo = TestDataBuilder.CreateMyCompanyInfo(c =>
        {
            c.Name = "Old Company";
            c.Email = "old@company.com";
        });
        await DbContext.MyCompanyInfo.AddAsync(existingInfo);
        await SaveChangesAndClearTracking();

        var updatedInfo = TestDataBuilder.CreateMyCompanyInfo(c =>
        {
            c.Name = "Updated Company";
            c.Email = "updated@company.com";
            c.Phone = "999-888-7777";
            c.Address = "123 New Street";
            c.LicenseNumber = "LIC123456";
            c.Zip = "12345";
        });

        // Act
        await _service.UpdateInfoAsync(updatedInfo);

        // Assert
        var saved = await DbContext.MyCompanyInfo.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Updated Company");
        saved.Email.Should().Be("updated@company.com");
        saved.Phone.Should().Be("999-888-7777");
        saved.Address.Should().Be("123 New Street");
        saved.LicenseNumber.Should().Be("LIC123456");
        saved.Zip.Should().Be("12345");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateInfoAsync_WhenExceptionOccurs_LogsErrorAndThrows()
    {
        // Arrange
        var newInfo = TestDataBuilder.CreateMyCompanyInfo();
        
        // Force an exception by disposing the context
        DbContext.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await _service.UpdateInfoAsync(newInfo));

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains("Error while updating company info")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateInfoAsync_WithNullValues_UpdatesOnlyProvidedFields()
    {
        // Arrange
        var existingInfo = TestDataBuilder.CreateMyCompanyInfo(c =>
        {
            c.Name = "Existing Company";
            c.Email = "existing@company.com";
            c.Phone = "111-222-3333";
        });
        await DbContext.MyCompanyInfo.AddAsync(existingInfo);
        await SaveChangesAndClearTracking();

        var partialUpdate = new MyCompanyInfo
        {
            Name = "Updated Name",
            Email = string.Empty, // Use empty string instead of null for non-nullable string
            Phone = existingInfo.Phone // Keep same phone
        };

        // Act
        await _service.UpdateInfoAsync(partialUpdate);

        // Assert
        var saved = await DbContext.MyCompanyInfo.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Updated Name");
        saved.Email.Should().Be(string.Empty);
        saved.Phone.Should().Be("111-222-3333");
    }
}

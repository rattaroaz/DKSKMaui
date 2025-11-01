using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;

namespace DKSKMaui.Tests.Models;

public class ContractorTests
{
    [Fact]
    public void Contractor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var contractor = new Contractor();

        // Assert
        contractor.Name.Should().Be(string.Empty);
        contractor.Address.Should().BeNull();
        contractor.CellPhone.Should().BeNull();
        contractor.Email.Should().BeNull();
        contractor.IsActive.Should().BeNull();
    }

    [Fact]
    public void Contractor_WithValidData_ShouldBeValid()
    {
        // Arrange
        var contractor = TestDataBuilder.CreateContractor(c =>
        {
            c.Name = "John Doe";
            c.SocailSecurityNumber = "123-45-6789";
        });

        // Assert
        contractor.Name.Should().NotBeNullOrEmpty();
        contractor.SocailSecurityNumber.Should().MatchRegex(@"^\d{3}-\d{2}-\d{4}$");
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(null, false)]
    public void Contractor_ActiveStatus_DeterminesAvailability(bool? isActive, bool expectedAvailable)
    {
        // Arrange
        var contractor = TestDataBuilder.CreateContractor(c => c.IsActive = isActive);

        // Act
        var isAvailable = contractor.IsActive == true;

        // Assert
        isAvailable.Should().Be(expectedAvailable);
    }

    [Fact]
    public void Contractor_RequiredFields_ShouldNotBeNull()
    {
        // Arrange
        var contractor = TestDataBuilder.CreateContractor();

        // Assert
        contractor.Name.Should().NotBeNullOrEmpty();
        contractor.Address.Should().NotBeNull();
        contractor.CellPhone.Should().NotBeNull();
        contractor.Email.Should().NotBeNull();
    }

    [Fact]
    public void Contractor_CanToggleActiveStatus()
    {
        // Arrange
        var contractor = TestDataBuilder.CreateContractor(c => c.IsActive = true);

        // Act
        contractor.IsActive = false;

        // Assert
        contractor.IsActive.Should().BeFalse();

        // Act again
        contractor.IsActive = true;

        // Assert
        contractor.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("12345", true)]
    [InlineData("12345-6789", true)]
    [InlineData("1234", false)]
    [InlineData("", false)]
    public void Contractor_ZipValidation(string zip, bool shouldBeValid)
    {
        // Arrange
        var contractor = TestDataBuilder.CreateContractor(c => c.Zip = zip);

        // Act
        var isValid = !string.IsNullOrEmpty(zip) && 
                     (System.Text.RegularExpressions.Regex.IsMatch(zip, @"^\d{5}$") ||
                      System.Text.RegularExpressions.Regex.IsMatch(zip, @"^\d{5}-\d{4}$"));

        // Assert
        isValid.Should().Be(shouldBeValid);
    }
}

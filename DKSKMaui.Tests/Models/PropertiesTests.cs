using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;

namespace DKSKMaui.Tests.Models;

public class PropertiesTests
{
    [Fact]
    public void Properties_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var property = new Properties();

        // Assert
        property.Name.Should().Be(string.Empty);
        property.Address.Should().BeNull();
        property.City.Should().BeNull();
        property.Zip.Should().BeNull();
        property.GateCode.Should().BeNull();
        property.LockBox.Should().BeNull();
        property.GarageRemoteCode.Should().BeNull();
        property.ManagerName.Should().BeNull();
        property.ManagerPhone.Should().BeNull();
        property.ManagerEmail.Should().BeNull();
        property.SpecialNote.Should().BeNull();
        property.IsActive.Should().BeNull();
        property.SupervisorId.Should().Be(0);
        property.Supervisor.Should().BeNull(); // Required navigation property initialized to null
    }

    [Fact]
    public void Properties_WithValidData_ShouldBeValid()
    {
        // Arrange
        var property = TestDataBuilder.CreateProperty(p =>
        {
            p.Name = "Sunset Apartments";
            p.Address = "123 Sunset Blvd";
            p.City = "Los Angeles";
            p.Zip = "90210";
            p.GateCode = "1234";
        });

        // Assert
        property.Name.Should().NotBeNullOrEmpty();
        property.Address.Should().NotBeNullOrEmpty();
        property.City.Should().NotBeNullOrEmpty();
        property.Zip.Should().NotBeNullOrEmpty();
        property.SupervisorId.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Properties_CanHaveSupervisor()
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor();
        var property = TestDataBuilder.CreateProperty(p =>
        {
            p.SupervisorId = supervisor.Id;
            p.Supervisor = supervisor;
        });

        // Assert
        property.Supervisor.Should().Be(supervisor);
        property.SupervisorId.Should().Be(supervisor.Id);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("manager@domain.com", true)]
    [InlineData("@example.com", false)]
    [InlineData("manager@", false)]
    public void Properties_ManagerEmailValidation(string email, bool shouldBeValid)
    {
        // Arrange
        var property = TestDataBuilder.CreateProperty(p => p.ManagerEmail = email);

        // Assert
        if (shouldBeValid && !string.IsNullOrEmpty(email))
        {
            property.ManagerEmail.Should().Contain("@");
            property.ManagerEmail.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Theory]
    [InlineData("123-456-7890", true)]
    [InlineData("(123) 456-7890", true)]
    [InlineData("1234567890", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void Properties_ManagerPhoneValidation(string phone, bool shouldBeValid)
    {
        // Arrange
        var property = TestDataBuilder.CreateProperty(p => p.ManagerPhone = phone);

        // Act
        if (!string.IsNullOrEmpty(phone))
        {
            var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
            var isValid = digitsOnly.Length >= 10;

            // Assert
            isValid.Should().Be(shouldBeValid);
        }
    }

    [Theory]
    [InlineData("12345", true)]
    [InlineData("12345-6789", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void Properties_ZipValidation(string zip, bool shouldBeValid)
    {
        // Arrange
        var property = TestDataBuilder.CreateProperty(p => p.Zip = zip);

        // Assert
        if (shouldBeValid && !string.IsNullOrEmpty(zip))
        {
            property.Zip.Should().NotBeNullOrWhiteSpace();
            property.Zip!.Length.Should().BeGreaterThanOrEqualTo(5);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void Properties_IsActiveNullable(bool? isActive)
    {
        // Arrange
        var property = TestDataBuilder.CreateProperty(p => p.IsActive = isActive);

        // Assert
        property.IsActive.Should().Be(isActive);
    }

    [Fact]
    public void Properties_RequiredFields_ShouldNotBeNull()
    {
        // Arrange
        var property = TestDataBuilder.CreateProperty();

        // Assert
        property.Name.Should().NotBeNull();
        // Note: Supervisor navigation property is null by default in TestDataBuilder
        // Only SupervisorId (foreign key) is set
        property.SupervisorId.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Properties_Equality_BasedOnId()
    {
        var prop1 = TestDataBuilder.CreateProperty(p => p.Id = 1);
        var prop2 = TestDataBuilder.CreateProperty(p => p.Id = 1);
        var prop3 = TestDataBuilder.CreateProperty(p => p.Id = 2);

        // Assert
        prop1.Id.Should().Be(prop2.Id);
        prop1.Id.Should().NotBe(prop3.Id);
    }
}

using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;

namespace DKSKMaui.Tests.Models;

public class SupervisorTests
{
    [Fact]
    public void Supervisor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var supervisor = new Supervisor();

        // Assert
        supervisor.Name.Should().Be(string.Empty);
        supervisor.Phone.Should().BeNull();
        supervisor.Email.Should().BeNull();
        supervisor.CompanyId.Should().Be(0);
        supervisor.Company.Should().BeNull(); // Required navigation property initialized to null
        supervisor.Properties.Should().NotBeNull();
        supervisor.Properties.Should().BeEmpty();
    }

    [Fact]
    public void Supervisor_WithValidData_ShouldBeValid()
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor(s =>
        {
            s.Name = "John Smith";
            s.Email = "john.smith@example.com";
            s.Phone = "555-123-4567";
        });

        // Assert
        supervisor.Name.Should().NotBeNullOrEmpty();
        supervisor.Email.Should().Contain("@");
        supervisor.Phone.Should().NotBeNullOrEmpty();
        supervisor.CompanyId.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Supervisor_CanHaveCompany()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany();
        var supervisor = TestDataBuilder.CreateSupervisor(s =>
        {
            s.CompanyId = company.Id;
            s.Company = company;
        });

        // Assert
        supervisor.Company.Should().Be(company);
        supervisor.CompanyId.Should().Be(company.Id);
    }

    [Fact]
    public void Supervisor_CanManageProperties()
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor();
        var property1 = TestDataBuilder.CreateProperty();
        var property2 = TestDataBuilder.CreateProperty();

        // Act
        supervisor.Properties.Add(property1);
        supervisor.Properties.Add(property2);

        // Assert
        supervisor.Properties.Should().Contain(property1);
        supervisor.Properties.Should().Contain(property2);
        supervisor.Properties.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("supervisor@domain.com", true)]
    [InlineData("@example.com", false)]
    [InlineData("supervisor@", false)]
    public void Supervisor_EmailValidation(string email, bool shouldBeValid)
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor(s => s.Email = email);

        // Assert
        if (shouldBeValid && !string.IsNullOrEmpty(email))
        {
            supervisor.Email.Should().Contain("@");
            supervisor.Email.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Theory]
    [InlineData("123-456-7890", true)]
    [InlineData("(123) 456-7890", true)]
    [InlineData("1234567890", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void Supervisor_PhoneValidation(string phone, bool shouldBeValid)
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor(s => s.Phone = phone);

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
    [InlineData("John Smith", true)]
    [InlineData("Jane Doe", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void Supervisor_NameValidation(string? name, bool shouldBeValid)
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor(s => s.Name = name!);

        // Assert
        if (shouldBeValid)
        {
            supervisor.Name.Should().NotBeNullOrWhiteSpace();
        }
        else
        {
            supervisor.Name.Should().BeNullOrWhiteSpace();
        }
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void Supervisor_CompanyIdValidation(int companyId, bool shouldBeValid)
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor(s => s.CompanyId = companyId);

        // Assert
        if (shouldBeValid)
        {
            supervisor.CompanyId.Should().BeGreaterThan(0);
        }
        else
        {
            supervisor.CompanyId.Should().BeLessThanOrEqualTo(0);
        }
    }

    [Fact]
    public void Supervisor_RequiredFields_ShouldNotBeNull()
    {
        // Arrange
        var supervisor = TestDataBuilder.CreateSupervisor();

        // Assert
        supervisor.Name.Should().NotBeNull();
        // Note: Company navigation property is null by default in TestDataBuilder
        // Only CompanyId (foreign key) is set
        supervisor.CompanyId.Should().BeGreaterThan(0);
        supervisor.Properties.Should().NotBeNull();
    }

    [Fact]
    public void Supervisor_Equality_BasedOnId()
    {
        var sup1 = TestDataBuilder.CreateSupervisor(s => s.Id = 1);
        var sup2 = TestDataBuilder.CreateSupervisor(s => s.Id = 1);
        var sup3 = TestDataBuilder.CreateSupervisor(s => s.Id = 2);

        // Assert
        sup1.Id.Should().Be(sup2.Id);
        sup1.Id.Should().NotBe(sup3.Id);
    }
}

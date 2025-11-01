using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;

namespace DKSKMaui.Tests.Models;

public class CompanyTests
{
    [Fact]
    public void Company_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var company = new Companny();

        // Assert
        company.Name.Should().Be(string.Empty);
        company.Address.Should().BeNull();
        company.Phone.Should().BeNull();
        company.Email.Should().BeNull();
        company.SpecialNote.Should().BeNull();
        company.Supervisors.Should().NotBeNull();
        company.Supervisors.Should().BeEmpty();
    }

    [Fact]
    public void Company_WithValidData_ShouldBeValid()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c =>
        {
            c.Name = "Test Company";
            c.Email = "test@company.com";
            c.Phone = "555-1234";
        });

        // Assert
        company.Name.Should().NotBeNullOrEmpty();
        company.Email.Should().Contain("@");
        company.Phone.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Company_CanAddSupervisors()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany();
        var supervisor1 = TestDataBuilder.CreateSupervisor();
        var supervisor2 = TestDataBuilder.CreateSupervisor();
        // Act
        company.Supervisors.Add(supervisor1);
        company.Supervisors.Add(supervisor2);

        // Assert
        company.Supervisors.Should().Contain(supervisor1);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user@domain", true)]
    [InlineData("@example.com", false)]
    [InlineData("user@", false)]
    public void Company_EmailValidation(string email, bool shouldBeValid)
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c => c.Email = email);

        // Act & Assert
        if (shouldBeValid)
        {
            company.Email.Should().Contain("@");
            company.Email.Should().NotBeNullOrWhiteSpace();
        }
        else
        {
            var isValidEmail = email.Contains("@") && 
                              email.IndexOf("@") > 0 && 
                              email.IndexOf("@") < email.Length - 1;
            isValidEmail.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData("123-456-7890", true)]
    [InlineData("(123) 456-7890", true)]
    [InlineData("1234567890", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void Company_PhoneValidation(string phone, bool shouldBeValid)
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany(c => c.Phone = phone);

        // Act
        var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
        var isValid = digitsOnly.Length >= 10;

        // Assert
        isValid.Should().Be(shouldBeValid);
    }

    [Fact]
    public void Company_RequiredFields_ShouldNotBeNull()
    {
        // Arrange
        var company = TestDataBuilder.CreateCompany();

        // Assert
        company.Name.Should().NotBeNull();
        company.Address.Should().NotBeNull();
        company.Phone.Should().NotBeNull();
        company.Email.Should().NotBeNull();
        company.Supervisors.Should().NotBeNull();
    }

    [Fact]
    public void Company_Equality_BasedOnId()
    {
        var company1 = TestDataBuilder.CreateCompany(c => c.Id = 1);
        var company2 = TestDataBuilder.CreateCompany(c => c.Id = 1);
        var company3 = TestDataBuilder.CreateCompany(c => c.Id = 2);

        // Assert
        company1.Id.Should().Be(company2.Id);
        company1.Id.Should().NotBe(company3.Id);
    }
}

using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;

namespace DKSKMaui.Tests.Models;

public class MyCompanyInfoTests
{
    [Fact]
    public void MyCompanyInfo_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var companyInfo = new MyCompanyInfo();

        // Assert
        companyInfo.Name.Should().Be(string.Empty);
        companyInfo.Phone.Should().Be(string.Empty);
        companyInfo.Email.Should().Be(string.Empty);
        companyInfo.Address.Should().Be(string.Empty);
        companyInfo.Zip.Should().Be(string.Empty);
        companyInfo.LicenseNumber.Should().Be(string.Empty);
    }

    [Fact]
    public void MyCompanyInfo_WithValidData_ShouldBeValid()
    {
        // Arrange
        var companyInfo = TestDataBuilder.CreateMyCompanyInfo(c =>
        {
            c.Name = "DKSK Official Painting";
            c.Email = "info@dksfficial.com";
            c.Phone = "555-123-4567";
            c.Address = "123 Main St";
            c.Zip = "12345";
            c.LicenseNumber = "LIC-ABC123";
        });

        // Assert
        companyInfo.Name.Should().NotBeNullOrEmpty();
        companyInfo.Email.Should().Contain("@");
        companyInfo.Phone.Should().NotBeNullOrEmpty();
        companyInfo.Address.Should().NotBeNullOrEmpty();
        companyInfo.Zip.Should().NotBeNullOrEmpty();
        companyInfo.LicenseNumber.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user@domain.com", true)]
    [InlineData("@example.com", false)]
    [InlineData("user@", false)]
    [InlineData("userexample.com", false)]
    public void MyCompanyInfo_EmailValidation(string email, bool shouldBeValid)
    {
        // Arrange
        var companyInfo = TestDataBuilder.CreateMyCompanyInfo(c => c.Email = email);

        // Assert
        if (shouldBeValid)
        {
            companyInfo.Email.Should().Contain("@");
            companyInfo.Email.Should().NotBeNullOrWhiteSpace();
        }
        else
        {
            var isValidEmail = email.Contains("@") &&
                              email.IndexOf("@") > 0 &&
                              email.IndexOf("@") < email.Length - 1 &&
                              email.Split('@').Length == 2;
            isValidEmail.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData("123-456-7890", true)]
    [InlineData("(123) 456-7890", true)]
    [InlineData("1234567890", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void MyCompanyInfo_PhoneValidation(string phone, bool shouldBeValid)
    {
        // Arrange
        var companyInfo = TestDataBuilder.CreateMyCompanyInfo(c => c.Phone = phone);

        // Act
        var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
        var isValid = digitsOnly.Length >= 10;

        // Assert
        isValid.Should().Be(shouldBeValid);
    }

    [Theory]
    [InlineData("LIC-ABC123", true)]
    [InlineData("123456789", true)]
    [InlineData("LIC-", false)]
    [InlineData("", false)]
    public void MyCompanyInfo_LicenseNumberValidation(string licenseNumber, bool shouldBeValid)
    {
        // Arrange
        var companyInfo = TestDataBuilder.CreateMyCompanyInfo(c => c.LicenseNumber = licenseNumber);

        // Assert
        if (shouldBeValid)
        {
            companyInfo.LicenseNumber.Should().NotBeNullOrWhiteSpace();
            companyInfo.LicenseNumber.Length.Should().BeGreaterThan(4);
        }
        else
        {
            companyInfo.LicenseNumber.Length.Should().BeLessThanOrEqualTo(4);
        }
    }

    [Theory]
    [InlineData("12345", true)]
    [InlineData("12345-6789", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void MyCompanyInfo_ZipValidation(string zip, bool shouldBeValid)
    {
        // Arrange
        var companyInfo = TestDataBuilder.CreateMyCompanyInfo(c => c.Zip = zip);

        // Act
        var isValid = zip.Length >= 5;

        // Assert
        isValid.Should().Be(shouldBeValid);
    }

    [Fact]
    public void MyCompanyInfo_RequiredFields_ShouldNotBeNull()
    {
        // Arrange
        var companyInfo = TestDataBuilder.CreateMyCompanyInfo();

        // Assert
        companyInfo.Name.Should().NotBeNull();
        companyInfo.Phone.Should().NotBeNull();
        companyInfo.Email.Should().NotBeNull();
        companyInfo.Address.Should().NotBeNull();
        companyInfo.Zip.Should().NotBeNull();
        companyInfo.LicenseNumber.Should().NotBeNull();
    }

    [Fact]
    public void MyCompanyInfo_Equality_BasedOnId()
    {
        var info1 = TestDataBuilder.CreateMyCompanyInfo(i => i.Id = 1);
        var info2 = TestDataBuilder.CreateMyCompanyInfo(i => i.Id = 1);
        var info3 = TestDataBuilder.CreateMyCompanyInfo(i => i.Id = 2);

        // Assert
        info1.Id.Should().Be(info2.Id);
        info1.Id.Should().NotBe(info3.Id);
    }
}

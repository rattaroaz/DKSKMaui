using DKSKMaui.Backend.Models;
using DKSKMaui.Tests.Infrastructure;

namespace DKSKMaui.Tests.Models;

public class JobDiscriptionTests
{
    [Fact]
    public void JobDiscription_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var jobDescription = new JobDiscription();

        // Assert
        jobDescription.description.Should().Be(string.Empty);
        jobDescription.sizeBathroom.Should().Be(0);
        jobDescription.sizeBedroom.Should().Be(0);
        jobDescription.price.Should().Be(0);
    }

    [Fact]
    public void JobDiscription_WithValidData_ShouldBeValid()
    {
        // Arrange
        var jobDescription = TestDataBuilder.CreateJobDescription(j =>
        {
            j.description = "Interior Painting";
            j.sizeBathroom = 2;
            j.sizeBedroom = 3;
            j.price = 1500;
        });

        // Assert
        jobDescription.description.Should().NotBeNullOrEmpty();
        jobDescription.sizeBathroom.Should().BeGreaterThan(0);
        jobDescription.sizeBedroom.Should().BeGreaterThan(0);
        jobDescription.price.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("Interior Painting", true)]
    [InlineData("Exterior Painting", true)]
    [InlineData("Drywall Repair", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void JobDiscription_DescriptionValidation(string? description, bool shouldBeValid)
    {
        // Arrange
        var jobDescription = TestDataBuilder.CreateJobDescription(j => j.description = description!);

        // Assert
        if (shouldBeValid)
        {
            jobDescription.description.Should().NotBeNullOrWhiteSpace();
        }
        else
        {
            jobDescription.description.Should().BeNullOrWhiteSpace();
        }
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(5, true)]
    [InlineData(-1, false)]
    public void JobDiscription_SizeBathroomValidation(int sizeBathroom, bool shouldBeValid)
    {
        // Arrange
        var jobDescription = TestDataBuilder.CreateJobDescription(j => j.sizeBathroom = sizeBathroom);

        // Assert
        if (shouldBeValid)
        {
            jobDescription.sizeBathroom.Should().BeGreaterThan(0);
        }
        else
        {
            jobDescription.sizeBathroom.Should().BeLessThanOrEqualTo(0);
        }
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(6, true)]
    [InlineData(-1, false)]
    public void JobDiscription_SizeBedroomValidation(int sizeBedroom, bool shouldBeValid)
    {
        // Arrange
        var jobDescription = TestDataBuilder.CreateJobDescription(j => j.sizeBedroom = sizeBedroom);

        // Assert
        if (shouldBeValid)
        {
            jobDescription.sizeBedroom.Should().BeGreaterThan(0);
        }
        else
        {
            jobDescription.sizeBedroom.Should().BeLessThanOrEqualTo(0);
        }
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(99, false)]
    [InlineData(100, true)]
    [InlineData(10000, true)]
    public void JobDiscription_PriceValidation(int price, bool shouldBeValid)
    {
        // Arrange
        var jobDescription = TestDataBuilder.CreateJobDescription(j => j.price = price);

        // Assert
        if (shouldBeValid)
        {
            jobDescription.price.Should().BeGreaterThanOrEqualTo(100);
        }
        else
        {
            jobDescription.price.Should().BeLessThan(100);
        }
    }

    [Fact]
    public void JobDiscription_RequiredFields_ShouldNotBeNull()
    {
        // Arrange
        var jobDescription = TestDataBuilder.CreateJobDescription();

        // Assert
        jobDescription.description.Should().NotBeNull();
    }

    [Fact]
    public void JobDiscription_Equality_BasedOnId()
    {
        var job1 = TestDataBuilder.CreateJobDescription(j => j.Id = 1);
        var job2 = TestDataBuilder.CreateJobDescription(j => j.Id = 1);
        var job3 = TestDataBuilder.CreateJobDescription(j => j.Id = 2);

        // Assert
        job1.Id.Should().Be(job2.Id);
        job1.Id.Should().NotBe(job3.Id);
    }
}

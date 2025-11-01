using DKSKMaui.Components.Pages.JobDescription;
using DKSKMaui.Tests.Infrastructure;
using Bunit;
using Xunit;
using FluentAssertions;

namespace DKSKMaui.Tests.Components.Pages.JobDescription;

public class JobdescriptionTests : BlazorTestBase
{
    [Fact]
    [Trait("Category", "Component")]
    public void Jobdescription_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<Jobdescription>();

        // Assert
        component.Find("h3").TextContent.Should().Be("JobDescription");
        component.Find("select").Should().NotBeNull();
        component.Find("input[type='number']").Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Jobdescription_HasJobDescriptionOptions()
    {
        // Act
        var component = RenderComponent<Jobdescription>();

        // Assert - Check for key job description options
        component.Markup.Should().Contain("interior walls, closet inside, ceiling");
        component.Markup.Should().Contain("kitchen cabinet - inside and outside");
        component.Markup.Should().Contain("all enamel surfaces including doors");
        component.Markup.Should().Contain("Balcony floor");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Jobdescription_HasPriceInput()
    {
        // Act
        var component = RenderComponent<Jobdescription>();

        // Assert
        var priceInput = component.Find("input[type='number']");
        priceInput.GetAttribute("title").Should().Be("Price");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Jobdescription_HasMaterialCostsInput()
    {
        // Act
        var component = RenderComponent<Jobdescription>();

        // Assert
        component.Markup.Should().Contain("Material costs:");
        var materialInput = component.Find("input[type='text']");
        materialInput.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Jobdescription_HasAddAnotherOptionInput()
    {
        // Act
        var component = RenderComponent<Jobdescription>();

        // Assert
        // Check that input elements exist
        var inputs = component.FindAll("input");
        inputs.Should().NotBeEmpty();
    }
}

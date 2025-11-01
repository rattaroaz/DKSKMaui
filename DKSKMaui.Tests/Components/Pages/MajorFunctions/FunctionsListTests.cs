using DKSKMaui.Components.Pages.MajorFunctions;
using DKSKMaui.Tests.Infrastructure;
using Bunit;
using Xunit;
using FluentAssertions;

namespace DKSKMaui.Tests.Components.Pages.MajorFunctions;

public class FunctionsListTests : BlazorTestBase
{
    [Fact]
    [Trait("Category", "Component")]
    public void FunctionsList_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<FunctionsList>();

        // Assert
        component.Find("h3").TextContent.Should().Be("FunctionsList");
        component.Find("select").Should().NotBeNull();
        component.Find("input[type='button']").Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void FunctionsList_HasCorrectOptions()
    {
        // Act
        var component = RenderComponent<FunctionsList>();

        // Assert - Check for expected options in the markup
        component.Markup.Should().Contain("Great Properties, Inc");
        component.Markup.Should().Contain("Slave Driver Properties");
        component.Markup.Should().Contain("Usuck Management");
        component.Markup.Should().Contain("WTF Corp");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void FunctionsList_SubmitButtonHasCorrectText()
    {
        // Act
        var component = RenderComponent<FunctionsList>();

        // Assert
        var button = component.Find("input[type='button']");
        button.GetAttribute("value").Should().Be("Submit");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void FunctionsList_ComponentInitializesCorrectly()
    {
        // Act
        var component = RenderComponent<FunctionsList>();

        // Assert
        component.Instance.Should().NotBeNull();
        // Component should render without errors
        component.Markup.Should().NotBeNullOrEmpty();
    }
}

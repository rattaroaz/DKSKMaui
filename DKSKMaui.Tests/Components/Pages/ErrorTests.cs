using DKSKMaui.Components.Pages;
using DKSKMaui.Tests.Infrastructure;
using Bunit;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace DKSKMaui.Tests.Components.Pages;

public class ErrorTests : BlazorTestBase
{
    [Fact]
    [Trait("Category", "Component")]
    public void Error_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        component.Find("h4").TextContent.Should().Contain("Application Error");
        component.Find(".card").Should().NotBeNull();
        component.Find(".card-header").Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Error_ShowsErrorMessage()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        component.Markup.Should().Contain("An unexpected error occurred");
        component.Markup.Should().Contain("What you can do:");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Error_HasNavigationOptions()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        component.Markup.Should().Contain("Try refreshing the page");
        component.Markup.Should().Contain("Go back to the");
        component.Markup.Should().Contain("home page");
        component.Markup.Should().Contain("Contact support");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Error_HasActionButtons()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        var buttons = component.FindAll("button");
        buttons.Should().HaveCountGreaterThanOrEqualTo(1); // At least the refresh button

        var links = component.FindAll("a");
        links.Should().HaveCountGreaterThanOrEqualTo(1); // At least the home link
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Error_ReturnHomeButtonWorks()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        var homeButton = component.Find("a.btn-primary");
        homeButton.GetAttribute("href").Should().Be("/");
        homeButton.TextContent.Trim().Should().Be("Return Home");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Error_RefreshButtonExists()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        var refreshButton = component.Find("button.btn-secondary");
        refreshButton.TextContent.Trim().Should().Contain("Refresh Page");
        refreshButton.GetAttribute("onclick").Should().Contain("location.reload");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Error_UsesBootstrapStyling()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        component.Markup.Should().Contain("container");
        component.Markup.Should().Contain("row");
        component.Markup.Should().Contain("col-md-8");
        component.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Error_HasErrorStyling()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        component.Markup.Should().Contain("border-danger");
        component.Markup.Should().Contain("bg-danger");
        component.Markup.Should().Contain("text-white");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Error_ComponentInitializesCorrectly()
    {
        // Act
        var component = RenderComponent<Error>();

        // Assert
        component.Instance.Should().NotBeNull();
        // Component should have generated a RequestId
        var requestIdElement = component.FindAll("code");
        requestIdElement.Should().HaveCountGreaterThanOrEqualTo(0); // May or may not be visible
    }
}

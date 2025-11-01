using DKSKMaui.Components.Pages.Profile;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Infrastructure;
using Bunit;
using Bunit.TestDoubles;
using Radzen;
using Xunit;
using FluentAssertions;

namespace DKSKMaui.Tests.Components.Pages.ProfileTests;

public class ProfileTests : BlazorTestBase
{
    [Fact]
    [Trait("Category", "Component")]
    public void Profile_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<Profile>();

        // Assert
        component.Markup.Should().Contain("Profile");
        component.Find("div.rz-card").Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Profile_ShowsAuthenticationMessage()
    {
        // Act
        var component = RenderComponent<Profile>();

        // Assert - Check for the authentication removal message
        component.Markup.Should().Contain("authentication has been removed");
        component.Markup.Should().Contain("full access to all features");
    }


    [Fact]
    [Trait("Category", "Component")]
    public void Profile_ComponentInitializesCorrectly()
    {
        // Act
        var component = RenderComponent<Profile>();

        // Assert
        component.Instance.Should().NotBeNull();
        component.Markup.Should().NotBeNullOrEmpty();
    }
}

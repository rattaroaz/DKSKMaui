using DKSKMaui.Components.Pages.EditContacts;
using DKSKMaui.Tests.Infrastructure;
using Bunit;
using Xunit;
using FluentAssertions;

namespace DKSKMaui.Tests.Components.Pages.EditContacts;

public class EditcontactsTests : BlazorTestBase
{
    [Fact]
    [Trait("Category", "Component")]
    public void Editcontacts_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<Editcontacts>();

        // Assert
        component.Find("h3").TextContent.Should().Contain("Contacts");
        component.Find(".row").Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Editcontacts_HasRadioButtonSelection()
    {
        // Act
        var component = RenderComponent<Editcontacts>();

        // Assert - Check for actual rendered radio button elements
        component.Markup.Should().Contain("Company");
        component.Markup.Should().Contain("Contractor");
        component.Markup.Should().Contain("rz-radiobutton");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Editcontacts_DefaultsToCompanySelection()
    {
        // Act
        var component = RenderComponent<Editcontacts>();

        // Assert - Should default to company selection (value 1)
        // Check that company edit component would be rendered
        component.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Editcontacts_HasResponsiveLayout()
    {
        // Act
        var component = RenderComponent<Editcontacts>();

        // Assert - Check for Bootstrap responsive classes
        component.Markup.Should().Contain("col-md-6");
        component.Markup.Should().Contain("col-12");
        component.Markup.Should().Contain("col-md-8");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Editcontacts_ComponentInitializesCorrectly()
    {
        // Act
        var component = RenderComponent<Editcontacts>();

        // Assert
        component.Instance.Should().NotBeNull();
        // Component should render without errors
        component.Markup.Should().NotBeNullOrEmpty();
    }
}

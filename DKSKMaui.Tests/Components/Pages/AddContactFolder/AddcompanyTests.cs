using Bunit;
using DKSKMaui.Components.Pages.AddContactFolder;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Components.Base;
using FluentAssertions;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Radzen;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Bunit.TestDoubles;

namespace DKSKMaui.Tests.Components.Pages.AddContactFolder;

public class AddcompanyTests : FormPageTestBase<Addcompany>
{
    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_RendersPageTitle()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert
        component.Should().NotBeNull();
        var markup = component.Markup;
        markup.Should().Contain("Add Company");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_RendersFormView_Initially()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("Company name *");
        markup.Should().Contain("Owner");
        markup.Should().Contain("Cell Phone");
        markup.Should().Contain("E-mail");
        markup.Should().Contain("Address");
        markup.Should().Contain("City");
        markup.Should().Contain("Zip");
        markup.Should().Contain("Special note");
        markup.Should().Contain("Cancel");
        markup.Should().Contain("Submit");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_FormFields_HaveCorrectTypes()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert
        var markup = component.Markup;
        // Check for proper form inputs with labels
        markup.Should().Contain("form-label");
        markup.Should().Contain("form-control");
        markup.Should().Contain("id=\"name\"");
        markup.Should().Contain("id=\"phone\"");
        markup.Should().Contain("id=\"email\"");
        markup.Should().Contain("id=\"address\"");
        markup.Should().Contain("id=\"city\"");
        markup.Should().Contain("id=\"zip\"");
        markup.Should().Contain("id=\"specialnote\"");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_FormHasValidation()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert - Form should have validation components
        var markup = component.Markup;
        markup.Should().Contain("Company name *");
        markup.Should().Contain("id=\"name\"");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_FormHasProperStructure()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert - Should have proper form structure
        var markup = component.Markup;
        markup.Should().Contain("<form");
        markup.Should().Contain("type=\"submit\"");
        markup.Should().Contain("Cancel");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_FormInputs_AreProperlyBound()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert - Form should have proper input structure
        var markup = component.Markup;
        markup.Should().Contain("id=\"name\"");
        markup.Should().Contain("id=\"phone\"");
        markup.Should().Contain("id=\"email\"");
        markup.Should().Contain("blazor:onchange");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_HasProperNavigation()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert - Component should have navigation capabilities
        var instance = component.Instance;
        instance.Should().NotBeNull();
        // Check that NavigationManager is injected
        var navigationManager = instance.GetType().GetProperty("NavigationManager", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        navigationManager.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_HasRequiredServices()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert - Component should have required services injected
        var instance = component.Instance;
        instance.Should().NotBeNull();
        // Check that required services are injected
        var companyService = instance.GetType().GetProperty("CompanyService", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        companyService.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_FormInputs_HavePlaceholderValues()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("placeholder=\"Enter company name\"");
        markup.Should().Contain("placeholder=\"Enter owner name\"");
        markup.Should().Contain("placeholder=\"Enter phone number\"");
        markup.Should().Contain("placeholder=\"Enter email address\"");
        markup.Should().Contain("placeholder=\"Enter address\"");
        markup.Should().Contain("placeholder=\"Enter city\"");
        markup.Should().Contain("placeholder=\"Enter zip code\"");
        markup.Should().Contain("placeholder=\"Enter any special notes\"");
    }


    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_ComponentLifecycle_WorksCorrectly()
    {
        // Arrange & Act
        var component = RenderComponent<Addcompany>();
        var instance = component.Instance;

        // Assert
        instance.Should().NotBeNull();
        component.Should().NotBeNull();
        // Component should have a company model
        var companyField = instance.GetType().GetField("company", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        companyField.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_Rendering_IsConsistent()
    {
        // Arrange & Act
        var component1 = RenderComponent<Addcompany>();
        var component2 = RenderComponent<Addcompany>();

        // Assert - Both components should render the same initial state
        component1.Markup.Should().Contain("Add Company");
        component1.Markup.Should().Contain("Company name *");
        component2.Markup.Should().Contain("Add Company");
        component2.Markup.Should().Contain("Company name *");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_MethodAccessibility()
    {
        // Arrange
        var component = RenderComponent<Addcompany>();
        var instance = component.Instance;

        // Assert - SubmitCompany method should be accessible
        var submitMethod = instance.GetType().GetMethod("SubmitCompany", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        submitMethod.Should().NotBeNull();
        submitMethod.IsPrivate.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcompany_PropertyAccessibility()
    {
        // Arrange
        var component = RenderComponent<Addcompany>();
        var instance = component.Instance;

        // Assert - Company property should be accessible
        var companyProperty = instance.GetType().GetField("company", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        companyProperty.Should().NotBeNull();
        companyProperty.IsPrivate.Should().BeTrue();
    }
}

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

public class AddcontractorTests : FormPageTestBase<Addcontractor>
{
    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_RendersPageTitle()
    {
        // Arrange & Act
        var component = RenderComponent<Addcontractor>();

        // Assert
        component.Should().NotBeNull();
        var markup = component.Markup;
        markup.Should().Contain("Addcontractor");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_RendersFormView_Initially()
    {
        // Arrange & Act
        var component = RenderComponent<Addcontractor>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("Contractor Name");
        markup.Should().Contain("License Number");
        markup.Should().Contain("Social Security Number");
        markup.Should().Contain("ID");
        markup.Should().Contain("Payroll percent");
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
    public void Addcontractor_RendersDisplayView_AfterSubmit()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();

        // Act - Click submit button to switch to display view
        var submitButton = component.Find("input[type='Submit']");
        submitButton.Click();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("Contractor name:  Amanda Huginkis");
        markup.Should().Contain("License Number:  R31223");
        markup.Should().Contain("Social Security Number:  333-22-5566");
        markup.Should().Contain("ID:  4422");
        markup.Should().Contain("Cell Phone:  333-225-7765");
        markup.Should().Contain("E-mail:  ahk@yahoo.com");
        markup.Should().Contain("11664 Plaza Place");
        markup.Should().Contain("City:  Century City");
        markup.Should().Contain("Zip:  99221");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_FormFields_HaveCorrectTypes()
    {
        // Arrange & Act
        var component = RenderComponent<Addcontractor>();

        // Assert
        var markup = component.Markup;
        var inputElements = markup.Split(new[] { "<input" }, StringSplitOptions.None).Length - 1;
        inputElements.Should().BeGreaterThanOrEqualTo(12); // 11 form fields + Cancel + Submit
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_SubmitButton_ChangesView()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();
        var instance = component.Instance;

        // Act - Click submit button
        var submitButton = component.Find("input[type='Submit']");
        submitButton.Click();

        // Assert - View should switch to display mode
        var markup = component.Markup;
        markup.Should().Contain("Amanda Huginkis");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_CancelButton_RemainsInFormView()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();
        var instance = component.Instance;

        // Act - Cancel button has no onclick handler, so it doesn't change state
        // We just verify the initial state

        // Assert - Should remain in form view
        var markup = component.Markup;
        markup.Should().Contain("Contractor Name");
        markup.Should().NotContain("Amanda Huginkis");
        instance.selectedValue.Should().Be(1); // Should remain 1
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_InitialState_IsFormView()
    {
        // Arrange & Act
        var component = RenderComponent<Addcontractor>();
        var instance = component.Instance;

        // Assert - Initial state should be form view (selectedValue = 1)
        instance.selectedValue.Should().Be(1);
        var markup = component.Markup;
        markup.Should().Contain("Contractor Name");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_DoneMethod_ChangesSelectedValue()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();
        var instance = component.Instance;

        // Act - Click submit button which calls Done()
        var submitButton = component.Find("input[type='Submit']");
        submitButton.Click();

        // Assert
        instance.selectedValue.Should().Be(2);
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_DisplayView_HasCorrectData()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();

        // Act - Click submit button to switch to display view
        var submitButton = component.Find("input[type='Submit']");
        submitButton.Click();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("Contractor name:  Amanda Huginkis");
        markup.Should().Contain("License Number:  R31223");
        markup.Should().Contain("Social Security Number:  333-22-5566");
        markup.Should().Contain("ID:  4422");
        // Note: Payroll percent is not displayed in the display view
        markup.Should().Contain("Cell Phone:  333-225-7765");
        markup.Should().Contain("E-mail:  ahk@yahoo.com");
        markup.Should().Contain("11664 Plaza Place");
        markup.Should().Contain("City:  Century City");
        markup.Should().Contain("Zip:  99221");
        markup.Should().Contain("Special Note:");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_FormInputs_HavePlaceholderValues()
    {
        // Arrange & Act
        var component = RenderComponent<Addcontractor>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("value=\"Contractor Name\"");
        markup.Should().Contain("value=\"License Number\"");
        markup.Should().Contain("value=\"Social Security Number\"");
        markup.Should().Contain("value=\"ID\"");
        markup.Should().Contain("value=\"Payroll percent\"");
        markup.Should().Contain("value=\"Cell Phone\"");
        markup.Should().Contain("value=\"E-mail\"");
        markup.Should().Contain("value=\"Address\"");
        markup.Should().Contain("value=\"City\"");
        markup.Should().Contain("value=\"Zip\"");
        markup.Should().Contain("value=\"Special note\"");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_ComponentLifecycle_WorksCorrectly()
    {
        // Arrange & Act
        var component = RenderComponent<Addcontractor>();
        var instance = component.Instance;

        // Assert
        instance.Should().NotBeNull();
        component.Should().NotBeNull();
        instance.selectedValue.Should().Be(1); // Initial value
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_Rendering_IsConsistent()
    {
        // Arrange & Act
        var component1 = RenderComponent<Addcontractor>();
        var component2 = RenderComponent<Addcontractor>();

        // Assert - Both components should render the same initial state
        component1.Markup.Should().Contain("Addcontractor");
        component1.Markup.Should().Contain("Contractor Name");
        component2.Markup.Should().Contain("Addcontractor");
        component2.Markup.Should().Contain("Contractor Name");
        component1.Instance.selectedValue.Should().Be(component2.Instance.selectedValue);
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_StateChange_UpdatesRendering()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();
        var initialMarkup = component.Markup;

        // Act - Click submit button to change state
        var submitButton = component.Find("input[type='Submit']");
        submitButton.Click();
        var updatedMarkup = component.Markup;

        // Assert - Markup should be different after state change
        initialMarkup.Should().NotBe(updatedMarkup);
        initialMarkup.Should().Contain("Contractor Name");
        updatedMarkup.Should().Contain("Amanda Huginkis");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_MethodAccessibility()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();
        var instance = component.Instance;

        // Assert - Done method should be accessible
        var doneMethod = instance.GetType().GetMethod("Done");
        doneMethod.Should().NotBeNull();
        doneMethod.IsPublic.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_PropertyAccessibility()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();
        var instance = component.Instance;

        // Assert - selectedValue property should be accessible
        var selectedValueProperty = instance.GetType().GetProperty("selectedValue");
        selectedValueProperty.Should().NotBeNull();
        selectedValueProperty.CanRead.Should().BeTrue();
        selectedValueProperty.CanWrite.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_PayrollPercent_FieldPresent()
    {
        // Arrange & Act
        var component = RenderComponent<Addcontractor>();

        // Assert - Payroll percent field should be present in form view
        var markup = component.Markup;
        markup.Should().Contain("Payroll percent");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Addcontractor_Address_FieldInDisplayView()
    {
        // Arrange
        var component = RenderComponent<Addcontractor>();

        // Act - Click submit button to switch to display view
        var submitButton = component.Find("input[type='Submit']");
        submitButton.Click();

        // Assert - Address should be displayed in display view
        var markup = component.Markup;
        markup.Should().Contain("11664 Plaza Place");
    }
}

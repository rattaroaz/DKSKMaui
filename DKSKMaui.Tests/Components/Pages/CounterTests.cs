using Bunit;
using DKSKMaui.Components.Pages;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Components.Base;
using DKSKMaui.Tests.Infrastructure;
using FluentAssertions;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Radzen;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Bunit.TestDoubles;

namespace DKSKMaui.Tests.Components.Pages;

public class CounterTests : BlazorTestBase
{
    [Fact]
    [Trait("Category", "Component")]
    public void Counter_RendersPageTitle()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();

        // Assert
        component.Should().NotBeNull();
        var markup = component.Markup;
        markup.Should().Contain("Counter");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_InitialCount_IsZero()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("Current count: 0");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_RendersButton()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("Click me");
        markup.Should().Contain("btn btn-primary");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_ButtonClick_IncrementsCount()
    {
        // Arrange
        var component = RenderComponent<Counter>();
        var button = component.Find("button");

        // Act - Click the button
        button.Click();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("Current count: 1");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_MultipleClicks_IncrementsCorrectly()
    {
        // Arrange
        var component = RenderComponent<Counter>();
        var button = component.Find("button");

        // Act - Click multiple times
        button.Click();
        button.Click();
        button.Click();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("Current count: 3");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_CountDisplay_HasAccessibilityRole()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("role=\"status\"");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_HasProperPageDirective()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();

        // Assert - The component should render without errors and have basic structure
        var markup = component.Markup;
        markup.Should().Contain("<h1>Counter</h1>");
        markup.Should().Contain("Current count: 0");
        markup.Should().Contain("Click me");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_ComponentLifecycle_WorksCorrectly()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();
        var instance = component.Instance;

        // Assert
        instance.Should().NotBeNull();
        component.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_InitialState_IsCorrect()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();
        var instance = component.Instance;

        // Assert - Check the private field value through reflection
        var currentCountField = instance.GetType().GetField("currentCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        currentCountField.Should().NotBeNull();
        var fieldValue = currentCountField.GetValue(instance);
        fieldValue.Should().NotBeNull();
        var countValue = (int)fieldValue!;
        countValue.Should().Be(0);
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_IncrementMethod_Exists()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();
        var instance = component.Instance;

        // Assert
        var incrementMethod = instance.GetType().GetMethod("IncrementCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        incrementMethod.Should().NotBeNull();
        incrementMethod.IsPrivate.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_StateChange_UpdatesRendering()
    {
        // Arrange
        var component = RenderComponent<Counter>();
        var initialMarkup = component.Markup;
        var button = component.Find("button");

        // Act
        button.Click();
        var updatedMarkup = component.Markup;

        // Assert - Markup should be different after state change
        initialMarkup.Should().NotBe(updatedMarkup);
        initialMarkup.Should().Contain("Current count: 0");
        updatedMarkup.Should().Contain("Current count: 1");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_Button_OnClick_IsBound()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("blazor:onclick");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_CountDisplay_UpdatesInRealTime()
    {
        // Arrange
        var component = RenderComponent<Counter>();
        var button = component.Find("button");

        // Act & Assert - Count should update immediately after each click
        button.Click();
        component.Markup.Should().Contain("Current count: 1");

        button.Click();
        component.Markup.Should().Contain("Current count: 2");

        button.Click();
        component.Markup.Should().Contain("Current count: 3");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_PrivateField_IsAccessible()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();
        var instance = component.Instance;

        // Assert - Should be able to access private field for testing
        var field = instance.GetType().GetField("currentCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.Should().NotBeNull();
        field.FieldType.Should().Be(typeof(int));
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_Method_IsCallable()
    {
        // Arrange
        var component = RenderComponent<Counter>();
        var instance = component.Instance;
        var method = instance.GetType().GetMethod("IncrementCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method.Invoke(instance, null);

        // Assert
        var field = instance.GetType().GetField("currentCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fieldValue = field.GetValue(instance);
        fieldValue.Should().NotBeNull();
        var countValue = (int)fieldValue!;
        countValue.Should().Be(1);
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_BlazorSyntax_IsCorrect()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();

        // Assert
        var markup = component.Markup;
        // Verify the component renders with proper Blazor event binding
        markup.Should().Contain("blazor:onclick");
        // Verify it has proper HTML structure
        markup.Should().Contain("<h1>");
        markup.Should().Contain("<button");
    }

    [Fact]
    [Trait("Category", "Component")]
    [Trait("Category", "Component")]
    public void Counter_ButtonClick_UpdatesDisplay()
    {
        // Arrange
        var component = RenderComponent<Counter>();
        var button = component.Find("button");

        // Act - Click the button
        button.Click();

        // Assert - Counter should increment in the display
        var markup = component.Markup;
        markup.Should().Contain("Current count: 1");
        markup.Should().NotContain("Current count: 0");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Counter_HTMLStructure_IsValid()
    {
        // Arrange & Act
        var component = RenderComponent<Counter>();

        // Assert
        var markup = component.Markup;
        markup.Should().Contain("<h1>");
        markup.Should().Contain("</h1>");
        markup.Should().Contain("<p");
        markup.Should().Contain("</p>");
        markup.Should().Contain("<button");
        markup.Should().Contain("</button>");
    }
}

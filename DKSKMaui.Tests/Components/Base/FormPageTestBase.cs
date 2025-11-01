using Bunit;
using DKSKMaui.Components.Pages;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Infrastructure;
using FluentAssertions;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Radzen;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Bunit.TestDoubles;

namespace DKSKMaui.Tests.Components.Base;

/// <summary>
/// Base class for testing pages with form components
/// </summary>
public abstract class FormPageTestBase<TPage> : BlazorTestBase where TPage : ComponentBase
{
    protected Mock<MyCompanyInfoService> MyCompanyInfoServiceMock { get; private set; } = new();
    protected Mock<NotificationService> NotificationServiceMock { get; private set; } = new();
    protected Mock<GlobalStateService> GlobalStateServiceMock { get; private set; } = new();

    protected FormPageTestBase()
    {
        // Setup common mocks
        MyCompanyInfoServiceMock = new Mock<MyCompanyInfoService>();
        NotificationServiceMock = new Mock<NotificationService>();
        GlobalStateServiceMock = new Mock<GlobalStateService>();

        // Register mocks in DI container
        Services.AddScoped(_ => MyCompanyInfoServiceMock.Object);
        Services.AddScoped(_ => NotificationServiceMock.Object);
        Services.AddScoped(_ => GlobalStateServiceMock.Object);
    }

    /// <summary>
    /// Creates test company info data
    /// </summary>
    protected MyCompanyInfo CreateTestCompanyInfo()
    {
        return TestDataBuilder.CreateMyCompanyInfo();
    }

    /// <summary>
    /// Verifies that form fields are rendered correctly
    /// </summary>
    protected void VerifyFormFieldsRender(IRenderedComponent<TPage> component, params string[] expectedLabels)
    {
        var markup = component.Markup;
        foreach (var label in expectedLabels)
        {
            markup.Should().Contain(label);
        }
    }

    /// <summary>
    /// Verifies that text input fields are present
    /// </summary>
    protected void VerifyTextInputsRender(IRenderedComponent<TPage> component, int expectedCount = 1)
    {
        var markup = component.Markup;
        var inputCount = markup.Split(new[] { "<input", "rz-textbox" }, StringSplitOptions.None).Length - 1;
        inputCount.Should().BeGreaterOrEqualTo(expectedCount);
    }

    /// <summary>
    /// Verifies that submit/save buttons are rendered
    /// </summary>
    protected void VerifySubmitButtonRenders(IRenderedComponent<TPage> component, string buttonText = "Save")
    {
        var markup = component.Markup;
        markup.Should().Contain(buttonText);
    }

    /// <summary>
    /// Helper to simulate filling a form field
    /// </summary>
    protected async Task FillFormField<T>(IRenderedComponent<TPage> component, string fieldName, T value)
        where T : ComponentBase
    {
        // This is a generic helper - specific implementations will vary by component
        // Subclasses should implement specific field filling logic
        await Task.CompletedTask;
    }
}

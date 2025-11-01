using DKSKMaui.Components.Pages.NewJobs;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Bunit;
using Bunit.TestDoubles;
using Radzen;
using Microsoft.AspNetCore.Components;
using Xunit;
using FluentAssertions;

namespace DKSKMaui.Tests.Components.Pages.NewJobs;

public class NewjobsTests : BlazorTestBase
{
    private readonly JobDescriptionService _jobDescriptionService;
    private readonly NavigationManager _navigationManager;
    private readonly NotificationService _notificationService;
    private readonly DialogService _dialogService;

    public NewjobsTests()
    {
        _jobDescriptionService = Services.GetRequiredService<JobDescriptionService>();
        _navigationManager = Services.GetRequiredService<NavigationManager>();
        _notificationService = Services.GetRequiredService<NotificationService>();
        _dialogService = Services.GetRequiredService<DialogService>();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Newjobs_RendersCorrectly()
    {
        // Act
        var component = RenderComponent<Newjobs>();

        // Assert
        component.Find("h3").TextContent.Should().Contain("New Jobs");
        component.Find(".container").Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Newjobs_HasJobDescriptionHeader()
    {
        // Act
        var component = RenderComponent<Newjobs>();

        // Assert
        component.Markup.Should().Contain("Job Description");
        component.Markup.Should().Contain("Bedrooms");
        component.Markup.Should().Contain("Bathrooms");
        component.Markup.Should().Contain("Price");
        component.Markup.Should().Contain("Actions");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Newjobs_HasResponsiveLayout()
    {
        // Act
        var component = RenderComponent<Newjobs>();

        // Assert - Check for Bootstrap responsive classes
        component.Markup.Should().Contain("col-md-12");
        component.Markup.Should().Contain("col-md-6");
        component.Markup.Should().Contain("row");
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Newjobs_HasFormInputs()
    {
        // Act
        var component = RenderComponent<Newjobs>();

        // Assert - Check for form inputs
        // The component has buttons and input elements
        component.Markup.Should().Contain("Add Job");
        component.Markup.Should().Contain("Done");
        component.Markup.Should().Contain("rz-button");
    }

    [Fact]
    [Trait("Category", "Component")]
    public async Task Newjobs_LoadsOnInitialization()
    {
        // Act
        var component = RenderComponent<Newjobs>();
        await component.InvokeAsync(() => Task.Delay(100)); // Allow component to initialize

        // Assert - Component loads successfully
        component.Instance.Should().NotBeNull();
        component.Markup.Should().Contain("container");
    }
}

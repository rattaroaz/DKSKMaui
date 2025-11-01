using DKSKMaui.Components.Pages.Sales;
using DKSKMaui.Backend.Models;
using DKSKMaui.Backend.Services;
using DKSKMaui.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace DKSKMaui.Tests.Components.Pages;

public class SalesTests : BlazorTestBase
{
    [Fact]
    [Trait("Category", "Component")]
    public void Sales_ComponentCanBeInstantiated()
    {
        // Arrange - Setup additional JSRuntime mocks for Sales component
        JSInterop.SetupVoid("setPdfData", _ => true);
        JSInterop.SetupVoid("saveAsPDFFallback", _ => true);
        JSInterop.SetupVoid("saveAsExcel", _ => true);
        
        try
        {
            // Act - Just instantiate the component without rendering
            var component = new Sales();
            
            // Assert
            component.Should().NotBeNull();
            component.Should().BeOfType<Sales>();
        }
        catch (Exception ex)
        {
            // For debugging - this will show us what the actual error is
            throw new Exception($"Failed to instantiate Sales component: {ex.Message}", ex);
        }
    }

    [Fact]
    [Trait("Category", "Component")]
    public void Sales_ComponentHasCorrectPageDirective()
    {
        // Arrange
        var componentType = typeof(Sales);
        var routeAttribute = componentType.GetCustomAttributes(typeof(RouteAttribute), false)
            .FirstOrDefault() as RouteAttribute;
        
        // Assert
        routeAttribute.Should().NotBeNull();
        routeAttribute!.Template.Should().Be("/sales");
    }
}

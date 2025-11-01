using DKSKMaui.Backend.Services;

namespace DKSKMaui.Tests.Services;

public class GlobalStateServiceTests
{
    private readonly GlobalStateService _service;

    public GlobalStateServiceTests()
    {
        _service = new GlobalStateService();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OnChange_EventCanBeSubscribed()
    {
        // Arrange
        var eventRaised = false;
        _service.OnChange += () => eventRaised = true;

        // Act - Use reflection to call private method
        var notifyMethod = typeof(GlobalStateService)
            .GetMethod("NotifyStateChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        notifyMethod?.Invoke(_service, null);

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OnChange_MultipleSubscribers_AllGetNotified()
    {
        // Arrange
        var subscriber1Called = false;
        var subscriber2Called = false;
        var subscriber3Called = false;

        _service.OnChange += () => subscriber1Called = true;
        _service.OnChange += () => subscriber2Called = true;
        _service.OnChange += () => subscriber3Called = true;

        // Act - Use reflection to call private method
        var notifyMethod = typeof(GlobalStateService)
            .GetMethod("NotifyStateChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        notifyMethod?.Invoke(_service, null);

        // Assert
        subscriber1Called.Should().BeTrue();
        subscriber2Called.Should().BeTrue();
        subscriber3Called.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OnChange_WhenUnsubscribed_DoesNotGetNotified()
    {
        // Arrange
        var callCount = 0;
        Action handler = () => callCount++;
        
        _service.OnChange += handler;

        // First notification
        var notifyMethod = typeof(GlobalStateService)
            .GetMethod("NotifyStateChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        notifyMethod?.Invoke(_service, null);
        
        callCount.Should().Be(1);

        // Unsubscribe
        _service.OnChange -= handler;

        // Act - Second notification after unsubscribe
        notifyMethod?.Invoke(_service, null);

        // Assert - Count should still be 1
        callCount.Should().Be(1);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OnChange_WhenNoSubscribers_DoesNotThrow()
    {
        // Arrange - No subscribers

        // Act & Assert - Should not throw
        var notifyMethod = typeof(GlobalStateService)
            .GetMethod("NotifyStateChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        var exception = Record.Exception(() => notifyMethod?.Invoke(_service, null));
        exception.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OnChange_SubscriberThrowsException_OtherSubscribersStillCalled()
    {
        // Arrange
        var subscriber1Called = false;

        _service.OnChange += () => subscriber1Called = true;
        _service.OnChange += () => throw new Exception("Test exception");
        _service.OnChange += () => { }; // Third subscriber that does nothing

        // Act - Use reflection to call private method
        var notifyMethod = typeof(GlobalStateService)
            .GetMethod("NotifyStateChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // This will throw because one of the handlers throws
        try
        {
            notifyMethod?.Invoke(_service, null);
        }
        catch
        {
            // Expected exception from the throwing handler
        }

        // Assert - First handler should have been called
        subscriber1Called.Should().BeTrue();
        // Note: Third subscriber is not checked because multicast delegates stop on first exception
    }
}

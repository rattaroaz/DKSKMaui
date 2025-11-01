namespace DKSKMaui.Backend.Services;

public class GlobalStateService
{
    // Event to notify when the state changes
    public event Action OnChange;

    // Method to notify components when the state changes
    private void NotifyStateChanged() => OnChange?.Invoke();
}

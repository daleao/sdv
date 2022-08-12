namespace DaLion.Common.Events;

/// <summary>Interface for an event wrapper allowing dynamic enabling / disabling.</summary>
public interface IManagedEvent
{
    /// <summary>Whether this event is enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Whether this event is enabled for a specific screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    bool IsEnabledForScreen(int screenId);

    /// <summary>Enable this event on the current screen.</summary>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    bool Enable();

    /// <summary>Enable this event on the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    bool EnableForScreen(int screenId);

    /// <summary>Enable this event on the all screens.</summary>
    void EnableForAllScreens();

    /// <summary>Disable this event on the current screen.</summary>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    bool Disable();

    /// <summary>Disable this event on the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    bool DisableForScreen(int screenId);

    /// <summary>Disable this event on the all screens.</summary>
    void DisableForAllScreens();

    /// <summary>Reset this event's enabled state on all screens.</summary>
    void Reset();
}
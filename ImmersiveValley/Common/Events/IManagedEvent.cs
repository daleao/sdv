namespace DaLion.Common.Events;

/// <summary>Interface for an event wrapper allowing dynamic enabling / disabling.</summary>
public interface IManagedEvent
{
    /// <summary>Gets a value indicating whether determines whether this event is enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Determines whether this event is enabled for a specific screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event is enabled for the specified screen, otherwise <see langword="false"/>.</returns>
    bool IsEnabledForScreen(int screenId);

    /// <summary>Enables this event on the current screen.</summary>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    bool Enable();

    /// <summary>Enables this event on the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    bool EnableForScreen(int screenId);

    /// <summary>Enables this event on the all screens.</summary>
    void EnableForAllScreens();

    /// <summary>Disables this event on the current screen.</summary>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    bool Disable();

    /// <summary>Disables this event on the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    bool DisableForScreen(int screenId);

    /// <summary>Disables this event on the all screens.</summary>
    void DisableForAllScreens();

    /// <summary>Resets this event's enabled state on all screens.</summary>
    void Reset();
}

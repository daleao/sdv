namespace DaLion.Common.Events;

/// <summary>Interface for an event wrapper allowing dynamic enabling / disabling.</summary>
public interface IManagedEvent
{
    /// <summary>Whether this event is enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Whether this event is enabled for a specific splitscreen player.</summary>
    /// <param name="screenID">The player's screen id.</param>
    bool IsEnabledForScreen(int screenID);

    /// <summary>Enable this event on the current screen.</summary>
    /// <returns><see langword="true"> if the event's enabled status was changed, otherwise <see langword="false">.</returns>
    bool Enable();

    /// <summary>Disable this event on the current screen.</summary>
    /// <returns><see langword="true"> if the event's enabled status was changed, otherwise <see langword="false">.</returns>
    bool Disable();
}
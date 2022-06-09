namespace DaLion.Stardew.Alchemy.Framework.Events;

/// <summary>Interface for an event wrapper allowing dynamic enabling / disabling.</summary>
public interface IEvent
{
    /// <summary>Whether this event is enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Whether this event is enabled for a specific splitscreen player.</summary>
    /// <param name="screenId">The player's screen id.</param>
    bool IsEnabledForScreen(int screenId);

    /// <summary>Enable this event on the current screen.</summary>
    void Enable();

    /// <summary>Disable this event on the current screen.</summary>
    void Disable();
}
namespace DaLion.Stardew.Professions.Framework.Events;

/// <summary>Interface for an event wrapper allowing dynamic enabling/disabling of events.</summary>
internal interface IEvent
{
    public bool IsEnabled { get; }

    /// <summary>Enable this event on the current screen.</summary>
    public void Enable();

    /// <summary>Disable this event on the current screen.</summary>
    public void Disable();
}
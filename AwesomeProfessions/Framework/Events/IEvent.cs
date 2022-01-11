namespace TheLion.Stardew.Professions.Framework.Events;

/// <summary>Interface for an event wrapper allowing dynamic enabling/disabling of events.</summary>
internal interface IEvent
{
    /// <summary>Enable this event so that it is allowed to run.</summary>
    public void Enable();

    /// <summary>Disable this event, preventing it from running.</summary>
    public void Disable();
}
namespace DaLion.Stardew.Ponds.Framework.Events;

/// <summary>Interface for an event that can be hooked or unhooked.</summary>
internal interface IEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public void Hook();

    /// <summary>Unhook this event from the event listener.</summary>
    public void Unhook();
}
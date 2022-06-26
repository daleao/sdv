namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerContextReceived"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class PeerContextReceivedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected PeerContextReceivedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IMultiplayerEvents.PeerContextReceived"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerContextReceived(object? sender, PeerContextReceivedEventArgs e)
    {
        if (Hooked.Value) OnPeerContextReceivedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerContextReceived" />
    protected abstract void OnPeerContextReceivedImpl(object? sender, PeerContextReceivedEventArgs e);
}
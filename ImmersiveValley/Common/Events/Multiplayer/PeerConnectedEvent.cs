namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerConnected"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class PeerConnectedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected PeerConnectedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IMultiplayerEvents.PeerConnected"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (IsHooked) OnPeerConnectedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerConnected" />
    protected abstract void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e);
}
namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerDisconnected"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class PeerDisconnectedEvent : BaseEvent
{
    /// <inheritdoc cref="IMultiplayerEvents.PeerDisconnected"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e)
    {
        if (hooked.Value) OnPeerDisconnectedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerDisconnected" />
    protected abstract void OnPeerDisconnectedImpl(object sender, PeerDisconnectedEventArgs e);
}
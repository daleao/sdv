namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerContextReceived"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class PeerContextReceivedEvent : BaseEvent
{
    /// <inheritdoc cref="IMultiplayerEvents.PeerContextReceived"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
    {
        if (hooked.Value) OnPeerContextReceivedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerContextReceived" />
    protected abstract void OnPeerContextReceivedImpl(object sender, PeerContextReceivedEventArgs e);
}
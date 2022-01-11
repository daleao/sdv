using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer.PeerConnected;

internal abstract class PeerConnectedEvent : BaseEvent
{
    /// <summary>Raised after a connection from another player is approved by the game.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnPeerConnected(object sender, PeerConnectedEventArgs e)
    {
        if (enabled.Value) OnPeerConnectedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerConnected" />
    protected abstract void OnPeerConnectedImpl(object sender, PeerConnectedEventArgs e);
}
using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

internal abstract class PeerDisconnectedEvent : BaseEvent
{
    /// <summary>Raised after the connection to a player is severed.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e)
    {
        if (enabled.Value) OnPeerDisconnectedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerDisconnected" />
    protected abstract void OnPeerDisconnectedImpl(object sender, PeerDisconnectedEventArgs e);
}
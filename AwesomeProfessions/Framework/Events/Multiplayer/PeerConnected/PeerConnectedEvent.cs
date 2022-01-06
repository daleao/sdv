using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer.PeerConnected;

internal abstract class PeerConnectedEvent : BaseEvent
{
    /// <inheritdoc />
    public override void Hook()
    {
        ModEntry.ModHelper.Events.Multiplayer.PeerConnected += OnPeerConnected;
    }

    /// <inheritdoc />
    public override void Unhook()
    {
        ModEntry.ModHelper.Events.Multiplayer.PeerConnected -= OnPeerConnected;
    }

    /// <summary>Raised after a connection from another player is approved by the game.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public abstract void OnPeerConnected(object sender, PeerConnectedEventArgs e);
}
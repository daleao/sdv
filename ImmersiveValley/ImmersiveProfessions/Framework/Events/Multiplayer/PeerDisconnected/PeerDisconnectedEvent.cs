namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerDisconnected"/> allowing dynamic enabling / disabling.</summary>
internal abstract class PeerDisconnectedEvent : BaseEvent
{
    /// <inheritdoc cref="IMultiplayerEvents.PeerDisconnected"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e)
    {
        if (enabled.Value) OnPeerDisconnectedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerDisconnected" />
    protected abstract void OnPeerDisconnectedImpl(object sender, PeerDisconnectedEventArgs e);
}
namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerConnected"/> allowing dynamic enabling / disabling.</summary>
internal abstract class PeerConnectedEvent : BaseEvent
{
    /// <inheritdoc cref="IMultiplayerEvents.PeerConnected"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerConnected(object sender, PeerConnectedEventArgs e)
    {
        if (enabled.Value) OnPeerConnectedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerConnected" />
    protected abstract void OnPeerConnectedImpl(object sender, PeerConnectedEventArgs e);
}
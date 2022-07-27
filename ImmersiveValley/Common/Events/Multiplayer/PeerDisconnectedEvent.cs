namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerDisconnected"/> allowing dynamic enabling / disabling.</summary>
internal abstract class PeerDisconnectedEvent : ManagedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected PeerDisconnectedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IMultiplayerEvents.PeerDisconnected"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
    {
        if (IsEnabled) OnPeerDisconnectedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerDisconnected" />
    protected abstract void OnPeerDisconnectedImpl(object? sender, PeerDisconnectedEventArgs e);
}
namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerConnected"/> allowing dynamic enabling / disabling.</summary>
internal abstract class PeerConnectedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PeerConnectedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected PeerConnectedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Multiplayer.PeerConnected += this.OnPeerConnected;
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && base.IsEnabled;

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Multiplayer.PeerConnected -= this.OnPeerConnected;
    }

    /// <inheritdoc cref="IMultiplayerEvents.PeerConnected"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnPeerConnectedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnPeerConnected"/>
    protected abstract void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e);
}

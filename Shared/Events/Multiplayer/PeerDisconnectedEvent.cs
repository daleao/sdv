namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerDisconnected"/> allowing dynamic enabling / disabling.</summary>
public abstract class PeerDisconnectedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PeerDisconnectedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected PeerDisconnectedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Multiplayer.PeerDisconnected += this.OnPeerDisconnected;
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && base.IsEnabled;

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Multiplayer.PeerDisconnected -= this.OnPeerDisconnected;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IMultiplayerEvents.PeerDisconnected"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnPeerDisconnectedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnPeerDisconnected"/>
    protected abstract void OnPeerDisconnectedImpl(object? sender, PeerDisconnectedEventArgs e);
}

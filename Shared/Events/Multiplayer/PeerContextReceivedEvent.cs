namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerContextReceived"/> allowing dynamic enabling / disabling.</summary>
public abstract class PeerContextReceivedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PeerContextReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected PeerContextReceivedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Multiplayer.PeerContextReceived += this.OnPeerContextReceived;
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && base.IsEnabled;

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Multiplayer.PeerContextReceived -= this.OnPeerContextReceived;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IMultiplayerEvents.PeerContextReceived"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnPeerContextReceived(object? sender, PeerContextReceivedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnPeerContextReceivedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnPeerContextReceived"/>
    protected abstract void OnPeerContextReceivedImpl(object? sender, PeerContextReceivedEventArgs e);
}

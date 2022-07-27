namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerContextReceived"/> allowing dynamic enabling / disabling.</summary>
internal abstract class PeerContextReceivedEvent : ManagedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected PeerContextReceivedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IMultiplayerEvents.PeerContextReceived"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerContextReceived(object? sender, PeerContextReceivedEventArgs e)
    {
        if (IsEnabled) OnPeerContextReceivedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerContextReceived" />
    protected abstract void OnPeerContextReceivedImpl(object? sender, PeerContextReceivedEventArgs e);
}
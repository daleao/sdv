namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.ModMessageReceived"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ModMessageReceivedEvent : ManagedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ModMessageReceivedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (IsEnabled) OnModMessageReceivedImpl(sender, e);
    }

    /// <inheritdoc cref="OnModMessageReceived" />
    protected abstract void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e);
}
namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.ModMessageReceived"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ModMessageReceivedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ModMessageReceivedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && base.IsEnabled;

    /// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnModMessageReceivedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnModMessageReceived"/>
    protected abstract void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e);
}

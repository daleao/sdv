namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.ModMessageReceived"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ModMessageReceivedEvent : BaseEvent
{
    /// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        if (enabled.Value) OnModMessageReceivedImpl(sender, e);
    }

    /// <inheritdoc cref="OnModMessageReceived" />
    protected abstract void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e);
}
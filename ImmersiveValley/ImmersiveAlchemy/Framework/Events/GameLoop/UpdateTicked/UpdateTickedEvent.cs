namespace DaLion.Stardew.Alchemy.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicked"/> allowing dynamic enabling / disabling.</summary>
internal abstract class UpdateTickedEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnUpdateTickedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUpdateTicked" />
    protected abstract void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e);
}
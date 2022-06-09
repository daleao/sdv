namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.OneSecondUpdateTicked"/> allowing dynamic enabling / disabling.</summary>
internal abstract class OneSecondUpdateTickedEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnOneSecondUpdateTickedImpl(sender, e);
    }

    /// <inheritdoc cref="OnOneSecondUpdateTicked" />
    protected abstract void OnOneSecondUpdateTickedImpl(object sender, OneSecondUpdateTickedEventArgs e);
}
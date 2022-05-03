namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.Saving"/> allowing dynamic enabling / disabling.</summary>
internal abstract class SavingEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaving(object sender, SavingEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnSavingImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaving" />
    protected abstract void OnSavingImpl(object sender, SavingEventArgs e);
}
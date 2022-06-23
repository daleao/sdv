namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.Saving"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class SavingEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaving(object sender, SavingEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnSavingImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaving" />
    protected abstract void OnSavingImpl(object sender, SavingEventArgs e);
}
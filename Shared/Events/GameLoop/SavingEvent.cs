namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.Saving"/> allowing dynamic enabling / disabling.</summary>
internal abstract class SavingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SavingEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.Saving += this.OnSaving;
    }

    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaving(object? sender, SavingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnSavingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnSaving"/>
    protected abstract void OnSavingImpl(object? sender, SavingEventArgs e);
}

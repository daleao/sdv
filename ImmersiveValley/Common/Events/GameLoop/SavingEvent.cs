namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.Saving"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class SavingEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SavingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaving(object? sender, SavingEventArgs e)
    {
        if (IsHooked) OnSavingImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaving" />
    protected abstract void OnSavingImpl(object? sender, SavingEventArgs e);
}
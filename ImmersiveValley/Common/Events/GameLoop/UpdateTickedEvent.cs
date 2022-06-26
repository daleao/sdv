namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicked"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class UpdateTickedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected UpdateTickedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Hooked.Value || GetType().Name.StartsWith("Static")) OnUpdateTickedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUpdateTicked" />
    protected abstract void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e);
}
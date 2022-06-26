namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="ISpecializedEvents.LoadStageChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LoadStageChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LoadStageChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="ISpecializedEvents.LoadStageChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLoadStageChanged(object? sender, LoadStageChangedEventArgs e)
    {
        if (Hooked.Value) OnLoadStageChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLoadStageChanged" />
    protected abstract void OnLoadStageChangedImpl(object? sender, LoadStageChangedEventArgs e);
}
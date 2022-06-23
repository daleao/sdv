namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.DayEnding"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class DayEndingEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDayEnding(object sender, DayEndingEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnDayEndingImpl(sender, e);
    }

    /// <inheritdoc cref="OnDayEnding" />
    protected abstract void OnDayEndingImpl(object sender, DayEndingEventArgs e);
}
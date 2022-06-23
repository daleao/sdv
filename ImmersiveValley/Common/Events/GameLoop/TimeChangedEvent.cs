namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.TimeChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class TimeChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.TimeChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnTimeChanged(object sender, TimeChangedEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnTimeChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnTimeChanged" />
    protected abstract void OnTimeChangedImpl(object sender, TimeChangedEventArgs e);
}
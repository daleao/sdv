namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.DebrisListChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class DebrisListChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IWorldEvents.DebrisListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDebrisListChanged(object sender, DebrisListChangedEventArgs e)
    {
        if (hooked.Value) OnDebrisListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnDebrisListChanged" />
    protected abstract void OnDebrisListChangedImpl(object sender, DebrisListChangedEventArgs e);
}
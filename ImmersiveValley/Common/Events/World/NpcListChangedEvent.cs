namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.NpcListChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class NpcListChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IWorldEvents.NpcListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnNpcListChanged(object sender, NpcListChangedEventArgs e)
    {
        if (hooked.Value) OnNpcListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnNpcListChanged" />
    protected abstract void OnNpcListChangedImpl(object sender, NpcListChangedEventArgs e);
}
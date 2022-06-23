namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.ObjectListChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class ObjectListChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IWorldEvents.ObjectListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
    {
        if (hooked.Value) OnObjectListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnObjectListChanged" />
    protected abstract void OnObjectListChangedImpl(object sender, ObjectListChangedEventArgs e);
}
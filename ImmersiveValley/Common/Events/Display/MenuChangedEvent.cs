namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.MenuChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class MenuChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnMenuChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnMenuChanged" />
    protected abstract void OnMenuChangedImpl(object sender, MenuChangedEventArgs e);
}
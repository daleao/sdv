namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.LocationListChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LocationListChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IWorldEvents.LocationListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLocationListChanged(object sender, LocationListChangedEventArgs e)
    {
        if (hooked.Value) OnLocationListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLocationListChanged" />
    protected abstract void OnLocationListChangedImpl(object sender, LocationListChangedEventArgs e);
}
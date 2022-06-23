namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="ISpecializedEvents.UnvalidatedUpdateTicked"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class UnvalidatedUpdateTickedEvent : BaseEvent
{
    /// <inheritdoc cref="ISpecializedEvents.UnvalidatedUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUnvalidatedUpdateTicked(object sender, UnvalidatedUpdateTickedEventArgs e)
    {
        if (hooked.Value) OnUnvalidatedUpdateTickedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUnvalidatedUpdateTicked" />
    protected abstract void OnUnvalidatedUpdateTickedImpl(object sender, UnvalidatedUpdateTickedEventArgs e);
}
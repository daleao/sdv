namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.LocaleChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LocaleChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IContentEvents.LocaleChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnLocaleChanged(object sender, LocaleChangedEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnLocaleChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLocaleChanged" />
    protected abstract void OnLocaleChangedImpl(object sender, LocaleChangedEventArgs e);
}
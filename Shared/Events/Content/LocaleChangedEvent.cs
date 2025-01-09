﻿namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.LocaleChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class LocaleChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LocaleChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LocaleChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Content.LocaleChanged += this.OnLocaleChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Content.LocaleChanged -= this.OnLocaleChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnLocaleChanged"/>
    protected abstract void OnLocaleChangedImpl(object? sender, LocaleChangedEventArgs e);

    /// <inheritdoc cref="IContentEvents.LocaleChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnLocaleChanged(object? sender, LocaleChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnLocaleChangedImpl(sender, e);
        }
    }
}

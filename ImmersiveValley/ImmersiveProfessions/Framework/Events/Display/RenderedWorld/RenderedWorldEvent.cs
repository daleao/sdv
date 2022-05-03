namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedWorld"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderedWorldEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.RenderedWorld"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnRenderedWorldImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedWorld" />
    protected abstract void OnRenderedWorldImpl(object sender, RenderedWorldEventArgs e);
}
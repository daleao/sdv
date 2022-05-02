namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedHud"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderedHudEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.RenderedHud"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnRenderedHudImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedHud" />
    protected abstract void OnRenderedHudImpl(object sender, RenderedHudEventArgs e);
}
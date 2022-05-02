namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedActiveMenu"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderedActiveMenuEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.RenderedActiveMenu"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnRenderedActiveMenuImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedActiveMenu" />
    protected abstract void OnRenderedActiveMenuImpl(object sender, RenderedActiveMenuEventArgs e);
}
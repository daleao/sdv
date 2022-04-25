namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingHud"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderingHudEvent : BaseEvent
{
    /// <summary>Raised before the game draws anything to the screen in a draw tick, as soon as the sprite batch is opened.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderingHud(object sender, RenderingHudEventArgs e)
    {
        if (enabled.Value) OnRenderingHudImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderingHud" />
    protected abstract void OnRenderingHudImpl(object sender, RenderingHudEventArgs e);
}
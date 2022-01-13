using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.Display;

internal abstract class RenderedHudEvent : BaseEvent
{
    /// <summary>
    ///     Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered
    ///     to the screen.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        if (enabled.Value) OnRenderedHudImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedHud" />
    protected abstract void OnRenderedHudImpl(object sender, RenderedHudEventArgs e);
}
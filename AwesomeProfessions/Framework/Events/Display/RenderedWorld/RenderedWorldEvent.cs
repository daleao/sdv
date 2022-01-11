using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.Display.RenderedWorld;

internal abstract class RenderedWorldEvent : BaseEvent
{
    /// <summary>Raised after the game world is drawn to the sprite patch, before it's rendered to the screen.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        if (enabled.Value) OnRenderedWorldImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedWorld" />
    protected abstract void OnRenderedWorldImpl(object sender, RenderedWorldEventArgs e);
}
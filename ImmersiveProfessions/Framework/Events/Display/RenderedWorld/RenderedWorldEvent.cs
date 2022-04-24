namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedWorld"/> allowing dynamic enabling / disabling.</summary>
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
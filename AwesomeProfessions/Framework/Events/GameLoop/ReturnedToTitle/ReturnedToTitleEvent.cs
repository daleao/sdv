using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.ReturnedToTitle;

internal abstract class ReturnedToTitleEvent : BaseEvent
{
    /// <summary>Raised after the game returns to the title screen.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnReturnedToTitleImpl(sender, e);
    }

    /// <inheritdoc cref="OnReturnedToTitle" />
    protected abstract void OnReturnedToTitleImpl(object sender, ReturnedToTitleEventArgs e);
}
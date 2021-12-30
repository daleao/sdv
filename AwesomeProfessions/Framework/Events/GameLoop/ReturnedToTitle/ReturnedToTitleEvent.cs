using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal abstract class ReturnedToTitleEvent : BaseEvent
{
    /// <inheritdoc />
    public override void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
    }

    /// <inheritdoc />
    public override void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.ReturnedToTitle -= OnReturnedToTitle;
    }

    /// <summary>Raised after the game returns to the title screen.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public abstract void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e);
}
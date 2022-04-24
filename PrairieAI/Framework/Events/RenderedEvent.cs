namespace DaLion.Stardew.Prairie.Training.Framework.Events;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.Rendered"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class RenderedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.Display.Rendered += OnRendered;
        Log.D("[Prairie] Hooked Rendered event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.Display.Rendered -= OnRendered;
        Log.D("[Prairie] Unhooked Rendered event.");
    }

    /// <summary>Raised after the game draws to the sprite batch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnRendered(object sender, RenderedEventArgs e)
    {
        if (ModEntry.IsPlayingAbigailGame) Debug.DrawBorders(e.SpriteBatch);
    }
}
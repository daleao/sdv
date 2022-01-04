using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

[UsedImplicitly]
internal class DebugUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        // show FPS counter
        ModEntry.FpsCounter?.Update(Game1.currentGameTime);
    }
}
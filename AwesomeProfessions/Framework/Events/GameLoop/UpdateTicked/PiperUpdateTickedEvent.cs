using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class PiperUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.State.Value.SlimeContactTimer > 0 && Game1.shouldTimePass()) --ModEntry.State.Value.SlimeContactTimer;
    }
}
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class SuperModeCountdownUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (Game1.game1.IsActive && Game1.shouldTimePass() && e.IsMultipleOf(ModEntry.Config.SuperModeDrainFactor))
            --ModEntry.State.Value.SuperModeGaugeValue;
        if (ModEntry.State.Value.SuperModeGaugeValue <= 0) ModEntry.Subscriber.Unsubscribe(GetType());
    }
}
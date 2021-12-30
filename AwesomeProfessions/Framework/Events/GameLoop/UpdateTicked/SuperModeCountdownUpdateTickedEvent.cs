using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeCountdownUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (Game1.game1.IsActive && Game1.shouldTimePass() && e.IsMultipleOf(ModEntry.Config.SuperModeDrainFactor))
            --ModState.SuperModeGaugeValue;
        if (ModState.SuperModeGaugeValue <= 0) ModEntry.Subscriber.Unsubscribe(GetType());
    }
}
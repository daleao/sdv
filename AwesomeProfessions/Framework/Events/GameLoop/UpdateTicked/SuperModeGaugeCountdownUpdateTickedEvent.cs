using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop;

internal class SuperModeGaugeCountdownUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var amount = Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds /
                     (ModEntry.Config.SuperModeDrainFactor * 10);
        ModEntry.State.Value.SuperMode.Gauge.Countdown(amount);
    }
}
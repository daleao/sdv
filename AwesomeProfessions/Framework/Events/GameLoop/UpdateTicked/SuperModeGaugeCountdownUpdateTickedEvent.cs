namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

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
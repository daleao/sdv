namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

using SuperMode;

#endregion using directives

internal class SuperModeActiveUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.PlayerState.Value.SuperMode is PiperEubstance)
        {
            if (!ModEntry.PlayerState.Value.SuperfluidSlimes.Any())
            {
                Disable();
                return;
            }

            var amount = Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
            foreach (var piped in ModEntry.PlayerState.Value.SuperfluidSlimes) piped.Countdown(amount);
        }
        else
        {
            if (!Game1.currentLocation.characters.OfType<Monster>().Any())
            {
                ModEntry.PlayerState.Value.SuperMode.Deactivate();
                return;
            }

            var amount = Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds /
                         (ModEntry.Config.SuperModeDrainFactor * 10);
            ModEntry.PlayerState.Value.SuperMode.Countdown(amount);
        }
    }
}
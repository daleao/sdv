namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

internal class SuperModeActiveUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (!Game1.currentLocation.characters.OfType<Monster>().Any())
        {
            ModEntry.State.Value.SuperMode.Deactivate();
            return;
        }

        var amount = Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds /
                     (ModEntry.Config.SuperModeDrainFactor * 10);
        ModEntry.State.Value.SuperMode.Countdown(amount);
    }
}
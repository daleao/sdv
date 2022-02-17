namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class PiperEubstanceActiveUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (!ModEntry.State.Value.SuperfluidSlimes.Any())
        {
            Disable();
            return;
        }

        var amount = Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
        foreach (var piped in ModEntry.State.Value.SuperfluidSlimes) piped.Countdown(amount);
    }
}
namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

using Ultimate;

#endregion using directives

internal class UltimateCountdownUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass()) return;

        ModEntry.PlayerState.Value.RegisteredUltimate.Countdown(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds);
    }
}
namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

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
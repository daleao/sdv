namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class PiperUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        // countdown contact timer
        if (ModEntry.PlayerState.SlimeContactTimer > 0 && Game1.game1.IsActive && Game1.shouldTimePass())
            --ModEntry.PlayerState.SlimeContactTimer;
    }
}
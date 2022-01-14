namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

using Patches.Foraging;

#endregion using directives

internal class PiperUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.State.Value.SlimeContactTimer > 0 &&
            Game1ShouldTimePassPatch.Game1ShouldTimePassOriginal(Game1.game1, true))
            --ModEntry.State.Value.SlimeContactTimer;
    }
}
using StardewModdingAPI.Events;
using StardewValley;
using DaLion.Stardew.Professions.Framework.Patches.Foraging;

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

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
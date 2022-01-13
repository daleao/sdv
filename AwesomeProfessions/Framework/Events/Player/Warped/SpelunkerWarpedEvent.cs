using System;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Events.Player;

internal class SpelunkerWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        if (e.NewLocation is MineShaft newShaft && e.OldLocation is MineShaft oldShaft &&
            newShaft.mineLevel > oldShaft.mineLevel)
        {
            ++ModEntry.State.Value.SpelunkerLadderStreak;

            if (e.Player.HasPrestigedProfession("Spelunker"))
            {
                var player = e.Player;
                player.health = Math.Min(player.health + (int) (player.maxHealth * 0.05f), player.maxHealth);
                player.Stamina = Math.Min(player.Stamina + player.MaxStamina * 0.05f, player.MaxStamina);
            }

            ModEntry.EventManager.Enable(typeof(SpelunkerBuffDisplayUpdateTickedEvent));
        }
        else if (e.OldLocation is MineShaft)
        {
            ModEntry.State.Value.SpelunkerLadderStreak = 0;
            ModEntry.EventManager.Disable(typeof(SpelunkerBuffDisplayUpdateTickedEvent));
        }
    }
}
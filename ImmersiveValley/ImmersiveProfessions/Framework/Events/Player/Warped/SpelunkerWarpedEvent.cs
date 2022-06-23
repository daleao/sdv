namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using System;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley.Locations;

using Common.Events;
using GameLoop;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        if (e.NewLocation is MineShaft newShaft && e.OldLocation is MineShaft oldShaft &&
            newShaft.mineLevel > oldShaft.mineLevel)
        {
            ++ModEntry.PlayerState.SpelunkerLadderStreak;

            if (e.Player.HasProfession(Profession.Spelunker, true))
            {
                var player = e.Player;
                player.health = Math.Min(player.health + (int) (player.maxHealth * 0.025f), player.maxHealth);
                player.Stamina = Math.Min(player.Stamina + player.MaxStamina * 0.01f, player.MaxStamina);
            }

            ModEntry.EventManager.Hook<SpelunkerUpdateTickedEvent>();
        }
        else if (e.NewLocation is not MineShaft && e.OldLocation is MineShaft)
        {
            ModEntry.PlayerState.SpelunkerLadderStreak = 0;
            ModEntry.EventManager.Hook<SpelunkerUpdateTickedEvent>();
        }
    }
}
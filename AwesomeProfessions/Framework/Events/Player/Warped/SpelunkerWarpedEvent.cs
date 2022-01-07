using StardewModdingAPI.Events;
using StardewValley.Locations;
using System;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.Player.Warped;

internal class SpelunkerWarpedEvent : WarpedEvent
{
    private static readonly SpelunkerBuffDisplayUpdateTickedEvent SpelunkerUpdateTickedEvent = new();

    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;

        if (e.NewLocation is MineShaft)
        {
            if (e.NewLocation.NameOrUniqueName == e.OldLocation.NameOrUniqueName) return;

            ++ModEntry.State.Value.SpelunkerLadderStreak;

            if (e.Player.HasPrestigedProfession("Spelunker"))
            {
                var player = e.Player;
                player.health = Math.Min(player.health + (int)(player.maxHealth * 0.05f), player.maxHealth);
                player.Stamina = Math.Min(player.Stamina + player.MaxStamina * 0.05f, player.MaxStamina);
            }

            ModEntry.Subscriber.SubscribeTo(SpelunkerUpdateTickedEvent);
        }
        else
        {
            ModEntry.State.Value.SpelunkerLadderStreak = 0;
            ModEntry.Subscriber.UnsubscribeFrom(SpelunkerUpdateTickedEvent.GetType());
        }
    }
}
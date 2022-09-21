namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using System;
using DaLion.Common.Events;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpelunkerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SpelunkerWarpedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation is MineShaft newShaft && e.OldLocation is MineShaft oldShaft &&
            newShaft.mineLevel > oldShaft.mineLevel)
        {
            ++ModEntry.State.SpelunkerLadderStreak;

            if (e.Player.HasProfession(Profession.Spelunker, true))
            {
                var player = e.Player;
                player.health = Math.Min(player.health + (int)(player.maxHealth * 0.025f), player.maxHealth);
                player.Stamina = Math.Min(player.Stamina + (player.MaxStamina * 0.01f), player.MaxStamina);
            }

            this.Manager.Enable<SpelunkerUpdateTickedEvent>();
        }
        else if (e.NewLocation is not MineShaft && e.OldLocation is MineShaft)
        {
            ModEntry.State.SpelunkerLadderStreak = 0;
            this.Manager.Enable<SpelunkerUpdateTickedEvent>();
        }
    }
}

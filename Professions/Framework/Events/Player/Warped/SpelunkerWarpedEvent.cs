namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerWarpedEvent : WarpedEvent
{
    private static int _previousMineLevel;

    /// <summary>Initializes a new instance of the <see cref="SpelunkerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpelunkerWarpedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Spelunker);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        if (e.OldLocation is MineShaft && e.Player.HasProfession(Profession.Spelunker, true))
        {
            if (e.NewLocation is MineShaft)
            {
                foreach (var debris in e.OldLocation.debris)
                {
                    if (debris.item is not null && Game1.random.NextBool())
                    {
                        State.SpelunkerUncollectedItems.Add(debris.item.getOne());
                    }
                }
            }
            else if (e.NewLocation.Name is "Mines" or "SkullCave")
            {
                Vector2[] offsets = [new Vector2(0f, 2f), new Vector2(1f, 2f), new Vector2(1f, 1f)];
                foreach (var item in State.SpelunkerUncollectedItems)
                {
                    Game1.createItemDebris(
                        item,
                        e.Player.getStandingPosition() + (offsets.Choose(Game1.random) * Game1.tileSize),
                        -1,
                        e.NewLocation);
                }

                State.SpelunkerUncollectedItems.Clear();
            }
        }

        if (e.NewLocation is not MineShaft && e.OldLocation is MineShaft)
        {
            State.SpelunkerLadderStreak = 0;
            _previousMineLevel = 0;
            return;
        }

        if (e.NewLocation is not MineShaft shaft || shaft.mineLevel <= _previousMineLevel)
        {
            return;
        }

        State.SpelunkerLadderStreak++;
        _previousMineLevel = shaft.mineLevel;
        if (!e.Player.HasProfession(Profession.Spelunker, true) || !shaft.IsTreasureOrSafeRoom())
        {
            return;
        }

        var player = e.Player;
        player.health = Math.Min(player.health + (int)(player.maxHealth * 0.05f), player.maxHealth);
        player.Stamina = Math.Min(player.Stamina + (player.MaxStamina * 0.05f), player.MaxStamina);
    }
}

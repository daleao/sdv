namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SpelunkerWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class SpelunkerWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static readonly Func<int, double> ItemRecoveryChance = x => 1 / (1 + Math.Exp(-0.02 * (x - 120)));
    private static int _previousMineLevel;

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Spelunker);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        var player = e.Player;
        var oldLocation = e.OldLocation;
        var newLocation = e.NewLocation;
        if (oldLocation is MineShaft && player.HasProfession(Profession.Spelunker, true))
        {
            if (oldLocation is MineShaft oldShaft)
            {
                foreach (var debris in oldLocation.debris)
                {
                    if (debris?.itemId?.Value is { } id && id.StartsWith("(O)") &&
                        Game1.random.NextBool())
                    {
                        State.SpelunkerUncollectedItems.Add((id, ItemRecoveryChance(oldShaft.mineLevel)));
                    }
                }
            }

            if (newLocation.Name is "Mine" or "SkullCave")
            {
                var mapWidth = newLocation.Map.Layers[0].LayerWidth;
                var mapHeight = newLocation.Map.Layers[0].LayerHeight;
                var spawnTiles = player.Tile.GetTwentyFourNeighbors(mapWidth, mapHeight).ToArray();
                foreach (var (id, chance) in State.SpelunkerUncollectedItems)
                {
                    if (Game1.random.NextBool(chance))
                    {
                        Game1.createItemDebris(
                            ItemRegistry.Create(id),
                            spawnTiles.Choose(Game1.random) * Game1.tileSize,
                            -1,
                            newLocation);
                    }
                }

                State.SpelunkerUncollectedItems.Clear();
            }
        }

        if (newLocation is not MineShaft && oldLocation is MineShaft)
        {
            State.SpelunkerLadderStreak = 0;
            State.SpelunkerCheckpoint = null;
            _previousMineLevel = 0;
            return;
        }

        if (newLocation is not MineShaft newShaft || newShaft.mineLevel <= _previousMineLevel)
        {
            return;
        }

        State.SpelunkerLadderStreak++;
        _previousMineLevel = newShaft.mineLevel;
        if (!newShaft.IsTreasureOrSafeRoom())
        {
            return;
        }

        var healed = (int)(player.maxHealth * 0.05f);
        player.health = Math.Min(player.health + healed, player.maxHealth);
        player.Stamina = Math.Min(player.Stamina + (player.MaxStamina * 0.05f), player.MaxStamina);
        newLocation.debris.Add(new Debris(
            healed,
            new Vector2(player.StandingPixel.X, player.StandingPixel.Y),
            Color.Lime,
            1f,
            player));
        Game1.playSound("healSound");
    }
}

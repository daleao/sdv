namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Xna;
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
    private static readonly Func<float, double> ItemRecoveryChance = x => 1 / (1 + Math.Exp(-0.02 * (x - 120)));
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

        State.SpelunkerClusterStreak = 0;
        State.SpelunkerLastStoneDestroyedAt = null;

        var player = e.Player;
        var oldLocation = e.OldLocation;
        var newLocation = e.NewLocation;
        var wasInMines = oldLocation is MineShaft;
        var isInMines = newLocation is MineShaft;
        if (player.HasProfession(Profession.Spelunker, true) && State.SpelunkerFlag is not null)
        {
            if (wasInMines && !ReferenceEquals(oldLocation, State.SpelunkerFlag.Location))
            {
                foreach (var debris in oldLocation.debris)
                {
                    if (debris?.itemId?.Value is { } id && id.StartsWith("(O)"))
                    {
                        State.SpelunkerUncollectedItems.Add((id, ItemRecoveryChance(State.SpelunkerLadderStreak)));
                    }
                }
            }

            if (isInMines && ReferenceEquals(newLocation, State.SpelunkerFlag.Location))
            {
                var mapWidth = newLocation.Map.Layers[0].LayerWidth;
                var mapHeight = newLocation.Map.Layers[0].LayerHeight;
                var spawnTiles = player.Tile.GetFourtyEightNeighbors(mapWidth, mapHeight).ToArray();
                for (var i = State.SpelunkerUncollectedItems.Count - 1; i >= 0; i--)
                {
                    var (id, chance) = State.SpelunkerUncollectedItems[i];
                    if (Random.Shared.NextBool(chance))
                    {
                        Game1.createItemDebris(
                            ItemRegistry.Create(id),
                            spawnTiles.Choose(Game1.random) * Game1.tileSize,
                            -1,
                            newLocation);
                    }

                    State.SpelunkerUncollectedItems.RemoveAt(i);
                }

                State.SpelunkerUncollectedItems.Clear();
            }
        }

        if (wasInMines && !isInMines)
        {
            State.SpelunkerLadderStreak = 0;
            _previousMineLevel = 0;
            return;
        }

        if (!isInMines)
        {
            return;
        }

        var newShaft = (MineShaft)newLocation;
        if (newShaft.mineLevel <= _previousMineLevel || newShaft.mineLevel == 1)
        {
            return;
        }

        State.SpelunkerLadderStreak = Math.Min(State.SpelunkerLadderStreak + 5, 100);
        this.Manager.Disable<SpelunkerTimeChangedEvent>();
        _previousMineLevel = newShaft.mineLevel;
    }
}

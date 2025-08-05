namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Professions.Framework.Hunting;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using xTile.Dimensions;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProspectorWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ProspectorWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static int _previousMineLevel;

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Prospector);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        State.ProspectorHunt ??= new ProspectorHunt();
        if (State.ProspectorHunt.IsActive)
        {
            State.ProspectorHunt.Fail();
        }

        var player = e.Player;
        var oldLocation = e.OldLocation;
        var newLocation = e.NewLocation;
        if (newLocation is not MineShaft newShaft)
        {
            _previousMineLevel = 0;
            return;
        }

        if (oldLocation is MineShaft && newShaft.mineLevel > _previousMineLevel)
        {
            State.ProspectorHunt.UpdateTriggerPool(0, 0, 1);
            _previousMineLevel = newShaft.mineLevel;
        }
        else if (oldLocation is not MineShaft)
        {
            _previousMineLevel = 0;
        }
        else if (newShaft.mineLevel < _previousMineLevel)
        {
            _previousMineLevel = newShaft.mineLevel;
        }

        if (!player.HasProfession(Profession.Prospector, true) || newLocation.currentEvent is not null ||
            newShaft.IsTreasureOrSafeRoom())
        {
            return;
        }

        var streak = Data.ReadAs<int>(player, DataKeys.LongestProspectorHuntStreak);
        var chance = Math.Atan(16d / 625d * streak);
        if (streak > 1 && Game1.random.NextBool(chance))
        {
            TrySpawnOreNodes(streak, newShaft);
        }
    }

    private static void TrySpawnOreNodes(int attempts, MineShaft shaft)
    {
        var r = Reflector.GetUnboundFieldGetter<MineShaft, Random>("mineRandom").Invoke(shaft);
        attempts = r.Next(Math.Min(attempts, 69)); // nice
        var count = 0;
        for (var i = 0; i < attempts; i++)
        {
            var tile = shaft.getRandomTile();
            if (!shaft.CanItemBePlacedHere(tile) || !shaft.isTileOnClearAndSolidGround(tile) ||
                shaft.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") is not null ||
                !shaft.isTileLocationOpen(new Location((int)tile.X, (int)tile.Y)) ||
                shaft.IsTileOccupiedBy(new Vector2(tile.X, tile.Y)) ||
                shaft.getTileIndexAt((int)tile.X, (int)tile.Y, "Back") == -1 ||
                shaft.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Type", "Back") == "Dirt")
            {
                continue;
            }

            shaft.placeAppropriateOreAt(new Vector2(tile.X, tile.Y));
            count++;
            if (count >= attempts / 2)
            {
                break;
            }
        }

        Log.D($"Prospector spawned {count} resource nodes.");
    }
}

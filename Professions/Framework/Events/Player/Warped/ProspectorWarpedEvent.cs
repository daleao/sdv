namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
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
    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Prospector);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer || e.NewLocation.farmers.Except(e.Player.Collect()).Any())
        {
            return;
        }

        State.ProspectorHunt ??= new ProspectorHunt();
        if (State.ProspectorHunt.IsActive)
        {
            State.ProspectorHunt.Fail();
        }

        if (!e.Player.HasProfession(Profession.Prospector) || e.NewLocation.currentEvent is not null ||
            e.NewLocation is not MineShaft shaft || shaft.IsTreasureOrSafeRoom())
        {
            return;
        }

        var streak = Data.ReadAs<int>(e.Player, DataKeys.ProspectorHuntStreak);
        var chance = Math.Atan(16d / 625d * streak);
        if (streak > 1 && Game1.random.NextBool(chance))
        {
            TrySpawnOreNodes(streak, shaft);
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

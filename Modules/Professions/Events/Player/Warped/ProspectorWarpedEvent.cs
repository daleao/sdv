namespace DaLion.Overhaul.Modules.Professions.Events.Player.Warped;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using xTile.Dimensions;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Prospector);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        var prospectorHunt = e.Player.Get_ProspectorHunt();
        if (prospectorHunt.IsActive)
        {
            prospectorHunt.Fail();
        }

        if (e.NewLocation.currentEvent is not null)
        {
            return;
        }

        switch (e.NewLocation)
        {
            case MineShaft shaft when !shaft.IsTreasureOrSafeRoom():
                var streak = e.Player.Read<int>(DataKeys.ProspectorHuntStreak);
                if (streak > 1)
                {
                    TrySpawnOreNodes(streak / 2, shaft);
                }

                prospectorHunt.TryStart(e.NewLocation);
                break;
            case VolcanoDungeon volcano when !volcano.IsTreasureOrSafeRoom():
                prospectorHunt.TryStart(e.NewLocation);
                break;
        }
    }

    private static void TrySpawnOreNodes(int attempts, MineShaft shaft)
    {
        var r = Reflector.GetUnboundFieldGetter<MineShaft, Random>(shaft, "mineRandom").Invoke(shaft);
        attempts = r.Next(Math.Min(attempts, 100));
        var count = 0;
        for (var i = 0; i < attempts; i++)
        {
            var tile = shaft.getRandomTile();
            if (!shaft.isTileLocationTotallyClearAndPlaceable(tile) || !shaft.isTileOnClearAndSolidGround(tile) ||
                shaft.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") is not null ||
                !shaft.isTileLocationOpen(new Location((int)tile.X, (int)tile.Y)) ||
                shaft.isTileOccupied(new Vector2(tile.X, tile.Y)) ||
                shaft.getTileIndexAt((int)tile.X, (int)tile.Y, "Back") == -1 ||
                shaft.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Type", "Back") == "Dirt")
            {
                continue;
            }

            shaft.placeAppropriateOreAt(new Vector2(tile.X, tile.Y));
            count++;
        }

        Log.D($"[Prospector]: Spawned {count} resource nodes.");
    }
}

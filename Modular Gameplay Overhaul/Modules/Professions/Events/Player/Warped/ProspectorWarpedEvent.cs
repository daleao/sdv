namespace DaLion.Overhaul.Modules.Professions.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Locations;

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
        var prospectorHunt = e.Player.Get_ProspectorHunt();
        if (prospectorHunt.IsActive)
        {
            prospectorHunt.Fail();
        }

        if (e.NewLocation.currentEvent is not null || e.NewLocation is not MineShaft shaft ||
            (!shaft.IsTreasureOrSafeRoom() && prospectorHunt.TryStart(e.NewLocation)))
        {
            return;
        }

        var streak = e.Player.Read<int>(DataFields.ProspectorHuntStreak);
        if (streak > 0)
        {
            TrySpawnOreNodes(streak * 2, shaft);
        }
    }

    private static void TrySpawnOreNodes(int amount, MineShaft shaft)
    {
        for (var i = 0; i < amount; i++)
        {
            var tile = shaft.getRandomTile();
            if (!shaft.isTileLocationTotallyClearAndPlaceable(tile) || !shaft.isTileOnClearAndSolidGround(tile) ||
                shaft.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null)
            {
                continue;
            }

            var ore = shaft.getAppropriateOre(tile);
            if (ore.ParentSheetIndex == 670)
            {
                ore.ParentSheetIndex = 668;
            }

            Utility.recursiveObjectPlacement(
                ore,
                (int)tile.X,
                (int)tile.Y,
                0.949999988079071,
                0.30000001192092896,
                shaft,
                "Dirt",
                ore.ParentSheetIndex == 668 ? 1 : 0,
                0.05000000074505806,
                ore.ParentSheetIndex != 668 ? 1 : 2);
        }
    }
}

namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class TreasureHuntCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="TreasureHuntCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal TreasureHuntCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "treasure_hunt", "treasure", "hunt" };

    /// <inheritdoc />
    public override string Documentation =>
        "Forcefully starts a treasure hunt with the target at the currently hovered tile, or changes the target tile if a hunt is already active.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var player = Game1.player;
        var location = player.currentLocation;
        var tile = Game1.currentCursorTile;
        if (location.IsOutdoors)
        {
            if (!player.HasProfession(Profession.Scavenger))
            {
                Log.W($"{player.Name} does not have the Scavenger profession.");
                return;
            }

            if (!location.IsTileValidForTreasure(tile))
            {
                Log.W("Cannot set Scavenger Hunt target at the current tile.");
                return;
            }

            var scavengerHunt = player.Get_ScavengerHunt();
            if (scavengerHunt.IsActive)
            {
                Game1.currentLocation.MakeTileDiggable(tile);
                ModHelper.Reflection.GetProperty<Vector2?>(scavengerHunt, "TreasureTile")
                    .SetValue(tile);
                ModHelper.Reflection.GetField<uint>(scavengerHunt, "elapsed").SetValue(0);
                Log.I("The Scavenger Hunt was reset.");
            }
            else
            {
                scavengerHunt.ForceStart(location, tile);
            }
        }
        else if (location is MineShaft)
        {
            if (!player.HasProfession(Profession.Prospector))
            {
                Log.W($"{player.Name} does not have the Prospector profession.");
                return;
            }

            if (!location.Objects.TryGetValue(tile, out var @object) || !@object.IsStone())
            {
                Log.W("Cannot set Prospector Hunt target at the current tile.");
                return;
            }

            var prospectorHunt = player.Get_ProspectorHunt();
            if (prospectorHunt.IsActive)
            {
                ModHelper.Reflection.GetProperty<Vector2?>(prospectorHunt, "TreasureTile")
                    .SetValue(tile);
                ModHelper.Reflection.GetField<int>(prospectorHunt, "Elapsed").SetValue(0);
                Log.I("The Prospector Hunt was reset.");
            }
            else
            {
                prospectorHunt.ForceStart(location, tile);
            }
        }
    }
}

namespace DaLion.Chargeable.Framework.Effects;

#region using directives

using System.Collections.Generic;
using DaLion.Chargeable.Framework.Configs;
using DaLion.Chargeable.Framework.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

#endregion using directives

/// <summary>Applies <see cref="Pickaxe"/> effects.</summary>
internal sealed class PickaxeEffect : IToolEffect
{
    /// <summary>Initializes a new instance of the <see cref="PickaxeEffect"/> class.</summary>
    /// <param name="config">The mod configs for the <see cref="Axe"/>.</param>
    public PickaxeEffect(PickaxeConfig config)
    {
        this.Config = config;
    }

    public PickaxeConfig Config { get; }

    /// <summary>Gets the <see cref="Pickaxe"/> upgrade levels needed to break supported resource clumps.</summary>
    /// <remarks>Derived from <see cref="ResourceClump.performToolAction"/>.</remarks>
    private IDictionary<int, int> UpgradeLevelsNeededForResource { get; } = new Dictionary<int, int>
    {
        [ResourceClump.meteoriteIndex] = Tool.gold, [ResourceClump.boulderIndex] = Tool.steel,
    };

    /// <inheritdoc />
    public bool Apply(
        Vector2 tile,
        SObject? tileObj,
        TerrainFeature tileFeature,
        Tool tool,
        GameLocation location,
        Farmer who)
    {
        // clear debris
        if (this.Config.ClearDebris && (tileObj?.IsStone() == true || tileObj?.IsWeed() == true))
        {
            return tool.UseOnTile(tile, location, who);
        }

        // clear placed paths & flooring
        if (this.Config.ClearFlooring && tileFeature is Flooring)
        {
            return tool.UseOnTile(tile, location, who);
        }

        // clear placed objects
        if (this.Config.ClearObjects && tileObj is not null)
        {
            return tool.UseOnTile(tile, location, who);
        }

        // break mine containers
        if (this.Config.BreakMineContainers && tileObj is not null)
        {
            return TryBreakContainer(tile, tileObj, tool, location);
        }

        // handle dirt
        if (tileFeature is HoeDirt dirt)
        {
            // clear tilled dirt
            if (dirt.crop is null && this.Config.ClearDirt)
            {
                return tool.UseOnTile(tile, location, who);
            }

            // clear crops
            if (dirt.crop is not null)
            {
                if (this.Config.ClearDeadCrops && dirt.crop.dead.Value)
                {
                    return tool.UseOnTile(tile, location, who);
                }

                if (this.Config.ClearLiveCrops && !dirt.crop.dead.Value)
                {
                    return tool.UseOnTile(tile, location, who);
                }
            }
        }

        // clear boulders / meteorites
        if (this.Config.BreakBouldersAndMeteorites)
        {
            var clump = location.GetResourceClumpCoveringTile(tile, who, out var applyTool);
            if (clump is not null &&
                (!this.UpgradeLevelsNeededForResource.TryGetValue(
                    clump.parentSheetIndex.Value,
                    out var requiredUpgradeLevel) || tool.UpgradeLevel >= requiredUpgradeLevel))
            {
                return applyTool!(tool);
            }
        }

        // harvest spawned mine objects
        if (!this.Config.HarvestMineSpawns || location is not MineShaft || tileObj?.IsSpawnedObject != true ||
            !location.checkAction(new Location((int)tile.X, (int)tile.Y), Game1.viewport, who))
        {
            return false;
        }

        who.CancelAnimation(
            FarmerSprite.harvestItemDown,
            FarmerSprite.harvestItemLeft,
            FarmerSprite.harvestItemRight,
            FarmerSprite.harvestItemUp);
        return true;
    }

    /// <summary>Breaks open a container using a tool, if applicable.</summary>
    /// <param name="tile">The tile position.</param>
    /// <param name="tileObj">The object on the tile.</param>
    /// <param name="tool">The tool selected by the player (if any).</param>
    /// <param name="location">The current location.</param>
    /// <returns><see langword="true"/> if the tool did break a container, otherwise <see langword="false"/>.</returns>
    private static bool TryBreakContainer(Vector2 tile, SObject tileObj, Tool tool, GameLocation location)
    {
        if (tileObj is BreakableContainer)
        {
            return tileObj.performToolAction(tool, location);
        }

        if (tileObj.bigCraftable.Value || tileObj.Name != "SupplyCrate" || tileObj is Chest ||
            !tileObj.performToolAction(tool, location))
        {
            return false;
        }

        tileObj.performRemoveAction(tile, location);
        Game1.currentLocation.Objects.Remove(tile);
        return true;
    }
}

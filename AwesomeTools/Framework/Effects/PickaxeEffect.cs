namespace DaLion.Stardew.Tools.Framework.Effects;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

using Configs;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Applies Pickaxe effects.</summary>
internal class PickaxeEffect : IEffect
{
    /// <summary>Construct an instance.</summary>
    public PickaxeEffect(PickaxeConfig config)
    {
        Config = config;
    }

    public PickaxeConfig Config { get; }

    /// <summary>The Pickaxe upgrade levels needed to break supported resource clumps.</summary>
    /// <remarks>Derived from <see cref="ResourceClump.performToolAction" />.</remarks>
    private IDictionary<int, int> UpgradeLevelsNeededForResource { get; } = new Dictionary<int, int>
    {
        [ResourceClump.meteoriteIndex] = Tool.gold,
        [ResourceClump.boulderIndex] = Tool.steel
    };

    /// <inheritdoc />
    public bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, Tool tool,
        GameLocation location, Farmer who)
    {
        // clear debris
        if (Config.ClearDebris && (tileObj.IsStone() || tileObj.IsWeed()))
            return tool.UseOnTile(tile, location, who);

        // break mine containers
        if (Config.BreakMineContainers && tileObj is not null)
            return TryBreakContainer(tile, tileObj, tool, location);

        // clear placed objects
        if (Config.ClearObjects && tileObj is not null)
            return tool.UseOnTile(tile, location, who);

        // clear placed paths & flooring
        if (Config.ClearFlooring && tileFeature is Flooring)
            return tool.UseOnTile(tile, location, who);

        // clear bushes
        if (Config.ClearBushes && tileFeature is Bush)
            return tool.UseOnTile(tile, location, who);

        // handle dirt
        if (tileFeature is HoeDirt dirt)
        {
            // clear tilled dirt
            if (dirt.crop is null && Config.ClearDirt)
                return tool.UseOnTile(tile, location, who);

            // clear crops
            if (dirt.crop is not null)
            {
                if (Config.ClearDeadCrops && dirt.crop.dead.Value)
                    return tool.UseOnTile(tile, location, who);

                if (Config.ClearLiveCrops && !dirt.crop.dead.Value)
                    return tool.UseOnTile(tile, location, who);
            }
        }

        // clear boulders / meteorites
        if (Config.BreakBouldersAndMeteorites)
        {
            var clump = location.GetResourceClumpCoveringTile(tile, who, out var applyTool);
            if (clump is not null &&
                (!UpgradeLevelsNeededForResource.TryGetValue(clump.parentSheetIndex.Value,
                    out var requiredUpgradeLevel) || tool.UpgradeLevel >= requiredUpgradeLevel)) return applyTool(tool);
        }

        // harvest spawned mine objects
        if (Config.HarvestMineSpawns && location is MineShaft && tileObj?.IsSpawnedObject == true &&
            location.checkAction(new((int)tile.X, (int)tile.Y), Game1.viewport, who))
        {
            who.CancelAnimation(FarmerSprite.harvestItemDown, FarmerSprite.harvestItemLeft,
                FarmerSprite.harvestItemRight, FarmerSprite.harvestItemUp);
            return true;
        }

        return false;
    }

    #region private methods

    /// <summary>Break open a container using a tool, if applicable.</summary>
    /// <param name="tile">The tile position</param>
    /// <param name="tileObj">The object on the tile.</param>
    /// <param name="tool">The tool selected by the player (if any).</param>
    /// <param name="location">The current location.</param>
    private static bool TryBreakContainer(Vector2 tile, SObject tileObj, Tool tool, GameLocation location)
    {
        if (tileObj is BreakableContainer)
            return tileObj.performToolAction(tool, location);

        if (tileObj.bigCraftable.Value || tileObj.Name != "SupplyCrate" || tileObj is Chest ||
            !tileObj.performToolAction(tool, location)) return false;

        tileObj.performRemoveAction(tile, location);
        Game1.currentLocation.Objects.Remove(tile);
        return true;
    }

    #endregion private methods
}
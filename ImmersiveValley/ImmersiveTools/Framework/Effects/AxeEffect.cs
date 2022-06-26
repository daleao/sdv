namespace DaLion.Stardew.Tools.Framework.Effects;

#region using directives

using Configs;
using Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Applies Axe effects.</summary>
internal class AxeEffect : IEffect
{
    /// <summary>Construct an instance.</summary>
    public AxeEffect(AxeConfig config)
    {
        Config = config;
    }

    public AxeConfig Config { get; }

    /// <summary>The Axe upgrade levels needed to break supported resource clumps.</summary>
    /// <remarks>Derived from <see cref="ResourceClump.performToolAction" />.</remarks>
    private IDictionary<int, int> UpgradeLevelsNeededForResource { get; } = new Dictionary<int, int>
    {
        [ResourceClump.stumpIndex] = Tool.copper,
        [ResourceClump.hollowLogIndex] = Tool.steel
    };

    /// <inheritdoc />
    public bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, Tool tool,
        GameLocation location, Farmer who)
    {
        // clear debris
        if (Config.ClearDebris && (tileObj.IsTwig() || tileObj.IsWeed()))
            return tool.UseOnTile(tile, location, who);

        // cut terrain features
        switch (tileFeature)
        {
            // cut non-fruit tree
            case Tree tree:
                return ShouldCut(tree) && tool.UseOnTile(tile, location, who);

            // cut fruit tree
            case FruitTree tree:
                return ShouldCut(tree) && tool.UseOnTile(tile, location, who);

            // cut bushes
            case Bush bush:
                return ShouldCut(bush) && tool.UseOnTile(tile, location, who);

            // clear crops
            case HoeDirt { crop: { } } dirt:
                if (Config.ClearDeadCrops && dirt.crop.dead.Value)
                    return tool.UseOnTile(tile, location, who);
                else if (Config.ClearLiveCrops && !dirt.crop.dead.Value)
                    return tool.UseOnTile(tile, location, who);

                break;
        }

        // cut resource stumps
        if (Config.ClearDebris || Config.CutGiantCrops)
        {
            var clump = location.GetResourceClumpCoveringTile(tile, who, out var applyTool);

            // giant crops
            if (Config.CutGiantCrops && clump is GiantCrop) return applyTool!(tool);

            // big stumps and fallen logs
            if (Config.ClearDebris && clump is not null &&
                UpgradeLevelsNeededForResource.ContainsKey(clump.parentSheetIndex.Value) && tool.UpgradeLevel >=
                UpgradeLevelsNeededForResource[clump.parentSheetIndex.Value])
                return applyTool!(tool);
        }

        // cut bushes in large terrain features
        if (Config.ClearBushes)
            if (location.largeTerrainFeatures.OfType<Bush>().Any(b => b.tilePosition.Value == tile))
                return tool.UseOnTile(tile, location, who);

        return false;
    }

    #region private methods

    /// <summary>Get whether a given tree should be chopped.</summary>
    /// <param name="tree">The tree to check.</param>
    private bool ShouldCut(Tree tree) =>
        tree.growthStage.Value switch
        {
            Tree.seedStage => Config.ClearTreeSeeds, // seed
            < Tree.treeStage => Config.ClearTreeSaplings, // sapling
            _ => tree.tapped.Value ? Config.CutTappedTrees : Config.CutGrownTrees // full-ground
        };

    /// <summary>Get whether a given tree should be chopped.</summary>
    /// <param name="tree">The tree to check.</param>
    private bool ShouldCut(FruitTree tree) =>
        tree.growthStage.Value switch
        {
            Tree.seedStage => Config.ClearFruitTreeSeeds, // seed
            < Tree.treeStage => Config.ClearFruitTreeSaplings, // sapling
            _ => Config.CutGrownFruitTrees // full-grown
        };

    /// <summary>Get whether bushes should be chopped.</summary>
    /// <param name="bush">A bush.</param>
    private bool ShouldCut(Bush bush) => Config.ClearBushes;

    #endregion private methods
}
namespace DaLion.Chargeable.Framework.Effects;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Chargeable.Framework.Configs;
using Framework.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

#endregion using directives

/// <summary>Applies <see cref="Axe"/> effects.</summary>
internal sealed class AxeEffect : IToolEffect
{
    /// <summary>Initializes a new instance of the <see cref="AxeEffect"/> class.</summary>
    /// <param name="config">The mod configs for the <see cref="Axe"/>.</param>
    public AxeEffect(AxeConfig config)
    {
        this.Config = config;
    }

    public AxeConfig Config { get; }

    /// <summary>Gets the <see cref="Axe"/> upgrade levels needed to break supported resource clumps.</summary>
    /// <remarks>Derived from <see cref="ResourceClump.performToolAction"/>.</remarks>
    private IDictionary<int, int> UpgradeLevelsNeededForResource { get; } = new Dictionary<int, int>
    {
        [ResourceClump.stumpIndex] = Tool.copper, [ResourceClump.hollowLogIndex] = Tool.steel,
    };

    /// <inheritdoc />
    public bool Apply(
        Vector2 tile, SObject? tileObj, TerrainFeature tileFeature, Tool tool, GameLocation location, Farmer who)
    {
        // clear debris
        if (this.Config.ClearDebris && (tileObj?.IsTwig() == true || tileObj?.IsWeed() == true))
        {
            return tool.UseOnTile(tile, location, who);
        }

        // cut terrain features
        switch (tileFeature)
        {
            // cut non-fruit tree
            case Tree tree:
                return this.ShouldCut(tree) && tool.UseOnTile(tile, location, who);

            // cut fruit tree
            case FruitTree tree:
                return this.ShouldCut(tree) && tool.UseOnTile(tile, location, who);

            // cut bushes
            case Bush bush:
                return this.ShouldCut(bush) && tool.UseOnTile(tile, location, who);

            // clear crops
            case HoeDirt { crop: { } } dirt:
                if (this.Config.ClearDeadCrops && dirt.crop.dead.Value)
                {
                    return tool.UseOnTile(tile, location, who);
                }

                if (this.Config.ClearLiveCrops && !dirt.crop.dead.Value)
                {
                    return tool.UseOnTile(tile, location, who);
                }

                break;
        }

        // cut resource stumps
        if (this.Config.ClearDebris || this.Config.CutGiantCrops)
        {
            var clump = location.GetResourceClumpCoveringTile(tile, who, out var applyTool);

            // giant crops
            if (this.Config.CutGiantCrops && clump is GiantCrop)
            {
                return applyTool!(tool);
            }

            // big stumps and fallen logs
            if (this.Config.ClearDebris && clump is not null &&
                this.UpgradeLevelsNeededForResource.ContainsKey(clump.parentSheetIndex.Value) && tool.UpgradeLevel >=
                this.UpgradeLevelsNeededForResource[clump.parentSheetIndex.Value])
            {
                return applyTool!(tool);
            }
        }

        // cut bushes in large terrain features
        if (this.Config.ClearBushes)
        {
            if (location.largeTerrainFeatures.OfType<Bush>().Any(b => b.tilePosition.Value == tile))
            {
                return tool.UseOnTile(tile, location, who);
            }
        }

        return false;
    }

    /// <summary>Determines whether the given <paramref name="tree"/> should be chopped.</summary>
    /// <param name="tree">The tree to check.</param>
    private bool ShouldCut(Tree tree)
    {
        return tree.growthStage.Value switch
        {
            Tree.seedStage => this.Config.ClearTreeSeeds, // seed
            < Tree.treeStage => this.Config.ClearTreeSaplings, // sapling
            _ => tree.tapped.Value ? this.Config.CutTappedTrees : this.Config.CutGrownTrees, // full-ground
        };
    }

    /// <summary>Determines whether the given <paramref name="fruitTree"/> should be chopped.</summary>
    /// <param name="fruitTree">The tree to check.</param>
    private bool ShouldCut(FruitTree fruitTree)
    {
        return fruitTree.growthStage.Value switch
        {
            Tree.seedStage => this.Config.ClearFruitTreeSeeds, // seed
            < Tree.treeStage => this.Config.ClearFruitTreeSaplings, // sapling
            _ => this.Config.CutGrownFruitTrees, // full-grown
        };
    }

    /// <summary>Determines whether <see cref="Bush"/>es should be chopped.</summary>
    /// <param name="bush">A bush.</param>
    /// <remarks>
    ///     The <see cref="Bush"/> instance is irrelevant, and only kept in the method signature to overload other
    ///     <c>ShouldCut</c> variations.
    /// </remarks>
    private bool ShouldCut(Bush bush)
    {
        return this.Config.ClearBushes;
    }
}

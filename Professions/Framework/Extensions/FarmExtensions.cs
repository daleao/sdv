namespace DaLion.Professions.Framework.Extensions;

#region using directives

using StardewValley.TerrainFeatures;

#endregion

/// <summary>Extensions for the <see cref="Farm"/> class.</summary>
internal static class FarmExtensions
{
    /// <summary>Counts the number of Green Rain Trees currently growing on the <paramref name="farm"/>.</summary>
    /// <param name="farm">The <see cref="Farm"/>.</param>
    /// <param name="matureOnly">Whether to consider only fully-grown trees.</param>
    /// <returns>The number of Green Rain Trees currently growing on the farm.</returns>
    internal static int CountGreenRainTrees(this Farm farm, bool matureOnly = true)
    {
        return farm.terrainFeatures.Values.Count(feature =>
            feature is Tree tree && (tree.growthStage.Value >= 5 || !matureOnly) &&
            tree.treeType.Value is Tree.greenRainTreeBushy or Tree.greenRainTreeFern or Tree.greenRainTreeLeafy);
    }
}

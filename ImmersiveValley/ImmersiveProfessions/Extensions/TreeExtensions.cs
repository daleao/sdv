namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using DaLion.Common.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
public static class TreeExtensions
{
    /// <summary>Determines whether the <paramref name="tree"/> satisfies all conditions to advance a stage.</summary>
    /// <param name="tree">The <see cref="Tree"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="tree"/> is not yet fully mature and environment conditions are suitable for growth, otherwise <see langword="false"/>.</returns>
    public static bool CanGrow(this Tree tree)
    {
        var tileLocation = tree.currentTileLocation;
        var environment = tree.currentLocation;
        if (Game1.GetSeasonForLocation(tree.currentLocation) == "winter" &&
            !tree.treeType.Value.IsIn(Tree.palmTree, Tree.palmTree2) &&
            !environment.CanPlantTreesHere(-1, (int)tileLocation.X, (int)tileLocation.Y) &&
            !tree.fertilized.Value)
        {
            return false;
        }

        var s = environment.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
        if (s is not null && s.IsIn("All", "Tree", "True"))
        {
            return false;
        }

        var growthRect = new Rectangle(
            (int)((tileLocation.X - 1f) * 64f),
            (int)((tileLocation.Y - 1f) * 64f),
            192,
            192);
        switch (tree.growthStage.Value)
        {
            case 4:
            {
                foreach (var (tile, feature) in environment.terrainFeatures.Pairs)
                {
                    if (feature is Tree otherTree && !otherTree.Equals(tree) &&
                        otherTree.growthStage.Value >= 5 &&
                        otherTree.getBoundingBox(tile).Intersects(growthRect))
                    {
                        return false;
                    }
                }

                break;
            }

            case 0 when environment.objects.ContainsKey(tileLocation):
                return false;
        }

        return true;
    }
}

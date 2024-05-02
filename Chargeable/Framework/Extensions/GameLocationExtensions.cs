namespace DaLion.Chargeable.Framework.Extensions;

#region using directives

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="GameLocation"/> class.</summary>
internal static class GameLocationExtensions
{
    /// <summary>Gets the <see cref="ResourceClump"/> which covers the given <paramref name="tile"/>, if any.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to check.</param>
    /// <param name="who">The current player.</param>
    /// <param name="applyTool">Applies a tool to the resource clump.</param>
    /// <returns>The <see cref="ResourceClump"/> located at <paramref name="tile"/> if any, otherwise <see langword="null"/>.</returns>
    internal static ResourceClump? GetResourceClumpCoveringTile(
        this GameLocation location, Vector2 tile, Farmer who, out Func<Tool, bool>? applyTool)
    {
        var tileArea = getAbsoluteTileArea(tile);

        // normal resource clumps
        foreach (var clump in location.resourceClumps)
        {
            if (!clump.getBoundingBox().Intersects(tileArea))
            {
                continue;
            }

            applyTool = tool => tool.UseOnTile(tile, location, who);
            return clump;
        }

        // FarmTypeManager resource clumps
        if (ModHelper.ModRegistry.IsLoaded("Esca.FarmTypeManager"))
        {
            foreach (var feature in location.largeTerrainFeatures)
            {
                if (feature.GetType().FullName != "FarmTypeManager.LargeResourceClump" ||
                    !feature.getBoundingBox().Intersects(tileArea))
                {
                    continue;
                }

                var clump = ModHelper.Reflection
                    .GetField<NetRef<ResourceClump>>(feature, "Clump").GetValue().Value;
                applyTool = tool => feature.performToolAction(tool, 0, tile);
                return clump;
            }
        }

        applyTool = null;
        return null;

        Rectangle getAbsoluteTileArea(Vector2 tile)
        {
            var (x, y) = tile * Game1.tileSize;
            return new Rectangle((int)x, (int)y, Game1.tileSize, Game1.tileSize);
        }
    }
}

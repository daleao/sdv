using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace TheLion.Stardew.Tools.Framework.Extensions;

public static class GameLocationExtensions
{
    /// <summary>Get the resource clumps in a given location.</summary>
    public static IEnumerable<ResourceClump> GetNormalResourceClumps(this GameLocation location)
    {
        IEnumerable<ResourceClump> clumps = location.resourceClumps;

        clumps = location switch
        {
            Forest {log: { }} forest => clumps.Concat(new[] {forest.log}),
            Woods woods when woods.stumps.Any() => clumps.Concat(woods.stumps),
            _ => clumps
        };

        return clumps;
    }

    /// <summary>Get the resource clump which covers a given tile, if any.</summary>
    /// <param name="tile">The tile to check.</param>
    /// <param name="who">The current player.</param>
    /// <param name="applyTool">Applies a tool to the resource clump.</param>
    public static ResourceClump GetResourceClumpCoveringTile(this GameLocation location, Vector2 tile, Farmer who,
        out Func<Tool, bool> applyTool)
    {
        var tileArea = tile.GetAbsoluteTileArea();

        // normal resource clumps
        foreach (var clump in location.GetNormalResourceClumps())
        {
            if (!clump.getBoundingBox(clump.tile.Value).Intersects(tileArea)) continue;

            applyTool = tool => tool.UseOnTile(tile, location, who);
            return clump;
        }

        // FarmTypeManager resource clumps
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("Esca.FarmTypeManager"))
            foreach (var feature in location.largeTerrainFeatures)
            {
                if (feature.GetType().FullName != "FarmTypeManager.LargeResourceClump" ||
                    !feature.getBoundingBox(feature.tilePosition.Value).Intersects(tileArea)) continue;

                var clump = ModEntry.ModHelper.Reflection
                    .GetField<NetRef<ResourceClump>>(feature, "Clump").GetValue().Value;
                applyTool = tool => feature.performToolAction(tool, 0, tile, location);
                return clump;
            }

        applyTool = null;
        return null;
    }
}
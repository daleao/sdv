namespace DaLion.Common.Classes;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using DaLion.Stardew.Common.Extensions;
using Microsoft.Xna.Framework;

#endregion using directives

public static class Tiles
{
    /// <summary>Search for region boundaries using a Flood Fill algorithm.</summary>
    /// <param name="origin">The starting point for the fill, as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the region.</param>
    /// <param name="height">The height of the region.</param>
    /// <param name="predicate">The boundary condition.</param>
    /// <returns>The set of points belonging to the region, as <see cref="Vector2"/>.</returns>
    public static IEnumerable<Vector2> FloodFill(Vector2 origin, int width, int height, Func<Vector2, bool> predicate)
    {
        if (origin.X <= 0) origin = new(origin.X + 1, origin.Y);
        else if (origin.Y <= 0) origin = new(origin.X, origin.Y + 1);
        else if (origin.X >= width - 1) origin = new(origin.X - 1, origin.Y);
        else if (origin.Y >= height - 1) origin = new(origin.X, origin.Y - 1);

        var result = new List<Vector2>();
        var visited = new HashSet<Vector2>();
        var toVisit = new Queue<Vector2>();
        toVisit.Enqueue(origin);
        while (toVisit.Any())
        {
            var tile = toVisit.Dequeue();
            if (!visited.Add(tile))
                continue;

            if (!predicate(tile)) continue;
            
            result.Add(tile);
            foreach (var neighbour in tile.GetEightNeighbours(width, height).Where(v => !visited.Contains(v)))
                toVisit.Enqueue(neighbour);
        }

        return result;
    }
}
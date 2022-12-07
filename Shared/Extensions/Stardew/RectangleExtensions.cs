namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Rectangle"/> struct.</summary>
public static class RectangleExtensions
{
    /// <summary>Enumerates all the tile within the <paramref name="rectangle"/>.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/>.</param>
    /// <returns>A <see cref="IEnumerable{Vector2}"/>, where <see cref="Vector2.X"/> and <see cref="Vector2.Y"/> represent the coordinates of tiles contained by the <paramref name="rectangle"/>.</returns>
    public static IEnumerable<Vector2> GetInnerTiles(this Rectangle rectangle)
    {
        for (var y = rectangle.Top / Game1.tileSize; y < rectangle.Bottom / Game1.tileSize; y++)
        {
            for (var x = rectangle.Left / Game1.tileSize; x < rectangle.Right / Game1.tileSize; x++)
            {
                yield return new Vector2(x, y);
            }
        }
    }
}

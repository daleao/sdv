namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Vector2"/> struct.</summary>
public static class Vector2Extensions
{
    /// <summary>Gets the <paramref name="tile"/>'s pixel position relative to the top-left corner of the map.</summary>
    /// <param name="tile">The tile.</param>
    /// <returns>A <see cref="Vector2"/> which represents the <c>X</c> and <c>Y</c> coordinates of the <paramref name="tile"/>'s pixel position.</returns>
    public static Vector2 GetPixelPosition(this Vector2 tile)
    {
        return (tile * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
    }

    /// <summary>
    ///     Gets a <see cref="Rectangle"/> representing the area in absolute pixels from the map's origin to the
    ///     <paramref name="tile"/>.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>A square <see cref="Rectangle"/> of side-length <see cref="Game1.tileSize"/> which represents the area of one game tile and originating at the <paramref name="tile"/>'s pixel position.</returns>
    public static Rectangle GetAbsoluteTileArea(this Vector2 tile)
    {
        var (x, y) = tile * Game1.tileSize;
        return new Rectangle((int)x, (int)y, Game1.tileSize, Game1.tileSize);
    }
}

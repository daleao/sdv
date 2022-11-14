namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using DaLion.Shared.Enums;
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

    /// <summary>Gets the next tile in the specified <paramref name="direction"/>.</summary>
    /// <param name="tile">The tile.</param>
    /// <param name="direction">A <see cref="FacingDirection"/>.</param>
    /// <returns>The next tile in the <paramref name="direction"/>.</returns>
    public static Vector2 GetNextTile(this Vector2 tile, FacingDirection direction)
    {
        return direction switch
        {
            FacingDirection.Up => tile + new Vector2(0, 1),
            FacingDirection.Right => tile + new Vector2(1, 0),
            FacingDirection.Down => tile + new Vector2(0, -1),
            FacingDirection.Left => tile + new Vector2(-1, 0),
            _ => Vector2.Zero,
        };
    }

    /// <summary>Gets the general <see cref="FacingDirection"/> pointed by the <see cref="Vector2"/>.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>The corresponding <see cref="FacingDirection"/>.</returns>
    public static FacingDirection ToFacingDirection(this Vector2 vector)
    {
        var (x, y) = vector;
        return Math.Abs(x) >= Math.Abs(y)
            ? x < 0 ? FacingDirection.Left : FacingDirection.Right
            : y > 0 ? FacingDirection.Down : FacingDirection.Up;
    }
}

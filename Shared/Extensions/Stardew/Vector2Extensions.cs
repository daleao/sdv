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
    /// <param name="direction">A <see cref="Direction"/>.</param>
    /// <returns>The next tile in the <paramref name="direction"/>.</returns>
    public static Vector2 GetNextTile(this Vector2 tile, Direction direction)
    {
        return direction switch
        {
            Direction.Up => tile + new Vector2(0, -1),
            Direction.Right => tile + new Vector2(1, 0),
            Direction.Down => tile + new Vector2(0, +1),
            Direction.Left => tile + new Vector2(-1, 0),
            _ => Vector2.Zero,
        };
    }

    /// <summary>Gets the next tile in the specified <paramref name="facingDirection"/>.</summary>
    /// <param name="tile">The tile.</param>
    /// <param name="facingDirection">An integer facing direction.</param>
    /// <returns>The next tile in the <paramref name="facingDirection"/>.</returns>
    public static Vector2 GetNextTile(this Vector2 tile, int facingDirection)
    {
        return GetNextTile(tile, (Direction)facingDirection);
    }

    /// <summary>Gets the next tile in the specified <paramref name="direction"/>.</summary>
    /// <param name="tile">The tile.</param>
    /// <param name="direction">A <see cref="Vector2"/> direction.</param>
    /// <returns>The next tile in the <paramref name="direction"/>.</returns>
    public static Vector2 GetNextTile(this Vector2 tile, Vector2 direction)
    {
        if (direction.Length() > 1f)
        {
            direction.Normalize();
        }

        return new Vector2((int)(tile.X + direction.X), (int)(tile.Y + direction.Y));
    }

    /// <summary>Gets the general <see cref="Direction"/> pointed by the <see cref="Vector2"/>.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>The corresponding <see cref="Direction"/>.</returns>
    public static Direction ToFacingDirection(this Vector2 vector)
    {
        var (x, y) = vector;
        return Math.Abs(x) >= Math.Abs(y)
            ? x < 0 ? Direction.Left : Direction.Right
            : y > 0 ? Direction.Down : Direction.Up;
    }

    /// <summary>
    ///     Finds the closest from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <param name="candidates">The candidates.</param>
    /// <param name="distance">The actual tile distance to the closest candidate found.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="vector"/>.</returns>
    public static Vector2? GetClosest(
        this Vector2 vector,
        IEnumerable<Vector2> candidates,
        out float distance,
        Func<Vector2, bool>? predicate = null)
    {
        predicate ??= _ => true;
        candidates = candidates.Where(c => predicate(c));
        Vector2? closest = null;
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = (vector - candidate).LengthSquared();
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        distance = (int)Math.Sqrt(distanceToClosest);
        return closest;
    }

    /// <summary>
    ///     Finds the closest target from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="vector"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <param name="candidates">The candidate <see typeparamref="T"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the tile coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The actual tile distance to the closest candidate found.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="vector"/>.</returns>
    public static T? GetClosest<T>(
        this Vector2 vector,
        IEnumerable<T> candidates,
        Func<T, Vector2> getPosition,
        out float distance,
        Func<T, bool>? predicate = null)
        where T : class
    {
        predicate ??= _ => true;
        candidates = candidates.Where(c => predicate(c));
        T? closest = null;
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = (vector - getPosition(candidate)).LengthSquared();
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        distance = (int)Math.Sqrt(distanceToClosest);
        return closest;
    }
}

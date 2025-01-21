namespace DaLion.Shared.Extensions.Xna;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="Point"/> struct.</summary>
public static class PointExtensions
{
    /// <summary>Finds the Manhattan (taxicab, or 4-connected) distance between two points.</summary>
    /// <param name="self">The <see cref="Point"/>.</param>
    /// <param name="other">Some other <see cref="Point"/>.</param>
    /// <returns>The Manhattan distance.</returns>
    public static int ManhattanDistance(this Point self, Point other)
    {
        return Math.Abs(self.X - other.X) + Math.Abs(self.Y - other.Y);
    }

    /// <summary>Finds the chessboard (Chebyshev) distance between two points.</summary>
    /// <param name="self">The <see cref="Point"/>.</param>
    /// <param name="other">Some other <see cref="Point"/>.</param>
    /// <returns>The Chessboard distance.</returns>
    public static int ChessboardDistance(this Point self, Point other)
    {
        return Math.Max(Math.Abs(self.X - other.X), Math.Abs(self.Y - other.Y));
    }

    /// <summary>Finds the midpoint between two points.</summary>
    /// <param name="self">The <see cref="Point"/>.</param>
    /// <param name="other">Some other <see cref="Point"/>.</param>
    /// <returns>The midpoint coordinates as <see cref="Vector2"/>.</returns>
    public static Vector2 Midpoint(this Point self, Point other)
    {
        return new Vector2(self.X + ((other.X - self.X) / 2f), self.Y + ((other.Y - self.Y) / 2f));
    }

    /// <summary>Draws a border of specified height and width starting at the <paramref name="point"/>.</summary>
    /// <param name="point">The <see cref="Point"/>.</param>
    /// <param name="height">The height of the border.</param>
    /// <param name="width">The width of the border.</param>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">The border thickness.</param>
    /// <param name="color">The border <see cref="Color"/>.</param>
    /// <param name="batch">A <see cref="SpriteBatch"/> to draw to.</param>
    public static void DrawBorder(
        this Point point, int height, int width, Texture2D pixel, int thickness, Color color, SpriteBatch batch)
    {
        var (x, y) = point;
        batch.Draw(pixel, new Rectangle(x, y, width, thickness), color); // top line
        batch.Draw(pixel, new Rectangle(x, y, thickness, height), color); // left line
        batch.Draw(pixel, new Rectangle(x + width - thickness, y, thickness, height), color); // right line
        batch.Draw(pixel, new Rectangle(x, y + height - thickness, width, thickness), color); // bottom line
    }

    /// <summary>Draws a border of specified height and width starting at the <paramref name="point"/>.</summary>
    /// <param name="point">The <see cref="Point"/>.</param>
    /// <param name="height">The height of the border.</param>
    /// <param name="width">The width of the border.</param>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">The border thickness.</param>
    /// <param name="color">The border <see cref="Color"/>.</param>
    /// <param name="batch">A <see cref="SpriteBatch"/> to draw to.</param>
    /// <param name="offset">An offset that should be applied to the point's position.</param>
    public static void DrawBorder(
        this Point point, int height, int width, Texture2D pixel, int thickness, Color color, SpriteBatch batch, Vector2 offset)
    {
        var (x, y) = point + offset.ToPoint();
        batch.Draw(pixel, new Rectangle(x, y, width, thickness), color); // top line
        batch.Draw(pixel, new Rectangle(x, y, thickness, height), color); // left line
        batch.Draw(pixel, new Rectangle(x + width - thickness, y, thickness, height), color); // right line
        batch.Draw(pixel, new Rectangle(x, y + height - thickness, width, thickness), color); // bottom line
    }

    /// <summary>Gets the 4-connected neighboring points in a given region.</summary>
    /// <param name="point">The center <see cref="Point"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the four-connected neighbors of the <paramref name="point"/>.</returns>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "Conciseness")]
    public static IEnumerable<Point> GetFourNeighbors(this Point point, int width, int height)
    {
        var (x, y) = point;
        if (x > 0) yield return new Point(x - 1, y);
        if (x < width - 1 ) yield return new Point(x + 1, y);
        if (y > 0) yield return new Point(x, y - 1);
        if (y < height - 1) yield return new Point(x, y + 1);
    }

    /// <summary>Gets the 8-connected neighboring points in a given region.</summary>
    /// <param name="point">The center <see cref="Point"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the eight-connected neighbors of the <paramref name="point"/>.</returns>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "Conciseness")]
    public static IEnumerable<Point> GetEightNeighbors(this Point point, int width, int height)
    {
        var (x, y) = point;
        if (x > 0)
        {
            yield return new Point(x - 1, y);
            if (y > 0) yield return new Point(x - 1, y - 1);
            if (y < height - 1) yield return new Point(x - 1, y + 1);
        }

        if (x < width - 1)
        {
            yield return new Point(x + 1, y);
            if (y > 0) yield return new Point(x + 1, y - 1);
            if (y < height - 1) yield return new Point(x + 1, y + 1);
        }

        if (y > 0) yield return new Point(x, y - 1);
        if (y < height - 1) yield return new Point(x, y + 1);
    }

    /// <summary>Gets the 24-connected neighboring points in a given region.</summary>
    /// <param name="point">The center point as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the twenty-four-connected neighbors of the <paramref name="point"/>.</returns>
    public static IEnumerable<Point> GetTwentyFourNeighbors(this Point point, int width, int height)
    {
        foreach (var neighbor in point.GetEightNeighbors(width, height))
        {
            yield return neighbor;
        }

        var (x, y) = point;
        if (y - 2 >= 0)
        {
            for (var i = -2; i <= 2; i++)
            {
                if ((x + i).IsIn(..width))
                {
                    yield return new Point(x + i, y - 2);
                }
            }
        }

        if (y + 2 < height)
        {
            for (var i = -2; i <= 2; i++)
            {
                if ((x + i).IsIn(..width))
                {
                    yield return new Point(x + i, y + 2);
                }
            }
        }

        if (x - 2 >= 0)
        {
            for (var j = -1; j <= 1; j++)
            {
                if ((y + j).IsIn(..height))
                {
                    yield return new Point(x - 2, y + j);
                }
            }
        }

        if (x + 2 < width)
        {
            for (var j = -1; j <= 1; j++)
            {
                if ((y + j).IsIn(..height))
                {
                    yield return new Point(x + 2, y + j);
                }
            }
        }
    }

    /// <summary>Searches for region boundaries using a Flood Fill algorithm.</summary>
    /// <param name="origin">The starting point for the fill, as a <see cref="Point"/>.</param>
    /// <param name="width">The width of the region.</param>
    /// <param name="height">The height of the region.</param>
    /// <param name="boundary">The boundary condition.</param>
    /// <returns>The list of <see cref="Point"/>s belonging to the enclosed region.</returns>
    public static IReadOnlyList<Point> FloodFill(this Point origin, int width, int height, Func<Point, bool> boundary)
    {
        var flooded = new List<Point>();
        var tested = new HashSet<Point>();
        var queue = new Queue<Point>();
        queue.Enqueue(origin);
        while (queue.Count > 0)
        {
            var tile = queue.Dequeue();
            if (tile.X < 0 || tile.Y < 0 || tile.X >= width || tile.Y >= height || !tested.Add(tile) ||
                !boundary(tile))
            {
                continue;
            }

            flooded.Add(tile);
            foreach (var neighbor in tile.GetEightNeighbors(width, height))
            {
                if (!tested.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return flooded;
    }
}

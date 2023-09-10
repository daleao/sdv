namespace DaLion.Chargeable.Framework;

#region using directives

using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Represents a circular grid of tiles.</summary>
public sealed class CircleTileGrid
{
    private readonly Vector2 _origin;
    private readonly bool[,] _outlineBoolArray;
    private readonly uint _radius;

    /// <summary>Initializes a new instance of the <see cref="CircleTileGrid"/> class.</summary>
    /// <param name="origin">The center tile of the circle in the world reference.</param>
    /// <param name="radius">The radius of the circle in tile units.</param>
    public CircleTileGrid(Vector2 origin, uint radius)
    {
        this._origin = origin;
        this._radius = radius;
        this._outlineBoolArray = this.GetOutline();
    }

    /// <summary>Gets enumerates all the tiles in the grid.</summary>
    public IEnumerable<Vector2> Tiles
    {
        get
        {
            // get the origin
            yield return this._origin;

            var center = new Vector2(this._radius, this._radius); // the center of the circle in the grid reference

            // get the central Axes
            for (var i = 0; i < (this._radius * 2) + 1; i++)
            {
                if (i == this._radius)
                {
                    continue;
                }

                yield return this._origin - center + new Vector2(i, this._radius);
                yield return this._origin - center + new Vector2(this._radius, i);
            }

            // loop over the first remaining quadrant and mirror matches 3 times
            for (var x = 0; x < this._radius; x++)
            {
                for (var y = 0; y < this._radius; y++)
                {
                    if (!this.Contains(new Point(x, y)))
                    {
                        continue;
                    }

                    yield return this._origin - center + new Vector2(y, x);
                    yield return this._origin - center + new Vector2(y, (2 * this._radius) - x);
                    yield return this._origin - center + new Vector2((2 * this._radius) - y, x);
                    yield return this._origin - center + new Vector2((2 * this._radius) - y, (2 * this._radius) - x);
                }
            }
        }
    }

    /// <summary>Gets enumerates only the outline tiles of the grid.</summary>
    public IEnumerable<Vector2> Outline
    {
        get
        {
            var center = new Vector2(this._radius, this._radius); // the center of the circle in the grid reference

            // get the central axis extremities
            yield return this._origin - center + new Vector2(0, this._radius);
            yield return this._origin - center + new Vector2(this._radius * 2, this._radius);
            yield return this._origin - center + new Vector2(this._radius, 0);
            yield return this._origin - center + new Vector2(this._radius, this._radius * 2);

            if (this._radius <= 1)
            {
                yield break;
            }

            // loop over the first remaining quadrant and mirror matches 3 times
            for (var x = 0; x < this._radius; x++)
            {
                for (var y = 0; y < this._radius; y++)
                {
                    if (!this._outlineBoolArray[x, y])
                    {
                        continue;
                    }

                    yield return this._origin - center + new Vector2(y, x);
                    yield return this._origin - center + new Vector2(y, (2 * this._radius) - x);
                    yield return this._origin - center + new Vector2((2 * this._radius) - y, x);
                    yield return this._origin - center + new Vector2((2 * this._radius) - y, (2 * this._radius) - x);
                }
            }
        }
    }

    /// <summary>Determines whether a point is contained within the circle by using ray casting.</summary>
    /// <param name="point">The point to be tested.</param>
    /// <returns><see langword="true"/> if the <paramref name="point"/> is within the bounds of the circle, otherwise <see langword="false"/>.</returns>
    /// <remarks>Remember that the center of the circle is located at (_radius, _radius).</remarks>
    public bool Contains(Point point)
    {
        // handle out of bounds
        if (point.X < 0 || point.Y < 0 || point.X > this._radius * 2 || point.Y > this._radius * 2)
        {
            return false;
        }

        // handle edge points
        if (point.X == 0 || point.Y == 0 || point.X == this._radius * 2 || point.Y == this._radius * 2)
        {
            return this._outlineBoolArray[point.Y, point.X];
        }

        // handle central Axes
        if (point.X == this._radius || point.Y == this._radius)
        {
            return true;
        }

        // handle remaining outline points
        if (this._outlineBoolArray[point.Y, point.X])
        {
            return true;
        }

        // mirror point into the first quadrant
        if (point.X > this._radius)
        {
            point.X = (int)this._radius - point.X;
        }

        if (point.Y > this._radius)
        {
            point.Y = (int)this._radius - point.Y;
        }

        // cast horizontal rays
        for (var i = point.X; i < this._radius; i++)
        {
            if (this._outlineBoolArray[point.Y, i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Gets a <see cref="string"/> representation of the circle grid.</summary>
    /// <returns>A <see cref="string"/> representation of the circle grid.</returns>
    public new string ToString()
    {
        var s = new StringBuilder().AppendLine();
        var height = this._outlineBoolArray.GetLength(0);
        var width = this._outlineBoolArray.GetLength(1);
        for (var i = 0; i < height; i++)
        {
            var first = 0;
            var last = width;
            for (var j = 0; j < width; j++)
            {
                if (!this._outlineBoolArray[i, j])
                {
                    continue;
                }

                first = j;
                break;
            }

            for (var j = width - 1; j >= 0; j--)
            {
                if (!this._outlineBoolArray[i, j])
                {
                    continue;
                }

                last = j;
                break;
            }

            var toggle = false;
            for (var j = 0; j < width; j++)
            {
                if (j == first || j == last + 1)
                {
                    toggle = !toggle;
                }

                s.Append(toggle ? 'x' : ' ').Append(' ');
            }

            s.AppendLine();
        }

        return s.ToString();
    }

    /// <summary>Creates the circle's outline as a <see cref="bool"/> array.</summary>
    /// <returns>An array of <see cref="bool"/> values, where <see langword="true"/> indicates that the circle's outline crosses over the corresponding tile.</returns>
    private bool[,] GetOutline()
    {
        var outline = new bool[(this._radius * 2) + 1, (this._radius * 2) + 1];
        var f = 1 - (int)this._radius;
        var ddFx = 1;
        var ddFy = -2 * (int)this._radius;
        var x = 0;
        var y = (int)this._radius;

        outline[this._radius, this._radius + this._radius] = true;
        outline[this._radius, this._radius - this._radius] = true;
        outline[this._radius + this._radius, this._radius] = true;
        outline[this._radius - this._radius, this._radius] = true;

        while (x < y)
        {
            if (f >= 0)
            {
                y--;
                ddFy += 2;
                f += ddFy;
            }

            x++;
            ddFx += 2;
            f += ddFx;

            outline[this._radius + x, this._radius + y] = true;
            outline[this._radius - x, this._radius + y] = true;
            outline[this._radius + x, this._radius - y] = true;
            outline[this._radius - x, this._radius - y] = true;
            outline[this._radius + y, this._radius + x] = true;
            outline[this._radius - y, this._radius + x] = true;
            outline[this._radius + y, this._radius - x] = true;
            outline[this._radius - y, this._radius - x] = true;
        }

        return outline;
    }
}

namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="Rectangle"/> struct.</summary>
public static class RectangleExtensions
{
    private static readonly Lazy<Texture2D> Pixel = new(() =>
    {
        var pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        pixel.SetData(new[] { Color.White });
        return pixel;
    });

    /// <summary>Enumerates all the tiles within the <paramref name="rectangle"/>.</summary>
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

    /// <summary>Highlights the tile defined by the specified <paramref name="rectangle"/> with the specified <paramref name="color"/>.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/>.</param>
    /// <param name="color">Border color.</param>
    /// <param name="batch"><see cref="SpriteBatch"/> to draw to.</param>
    /// <param name="offset">An optional offset.</param>
    public static void TileHighlight(
        this Rectangle rectangle, Color color, SpriteBatch batch, Vector2? offset = null)
    {
        offset ??= Vector2.Zero;
        batch.Draw(
            Game1.staminaRect,
            new Rectangle(
                rectangle.X + (int)offset.Value.X,
                rectangle.Y + (int)offset.Value.Y,
                rectangle.Width,
                rectangle.Height),
            color);
    }

    /// <summary>Highlights the border of the tile defined by the specified <paramref name="rectangle"/> with the specified <paramref name="color"/>.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/>.</param>
    /// <param name="color">Border color.</param>
    /// <param name="batch"><see cref="SpriteBatch"/> to draw to.</param>
    /// <param name="thickness">Border thickness.</param>
    public static void BorderHighlight(
        this Rectangle rectangle, Color color, SpriteBatch batch, int thickness = 4)
    {
        batch.Draw(
            Pixel.Value, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color.ChangeValue(0.8f)); // left line
        batch.Draw(
            Pixel.Value, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color.ChangeValue(0.8f)); // bottom line
        batch.Draw(
            Pixel.Value, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color); // top line
        batch.Draw(
            Pixel.Value, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color); // right line
    }
}

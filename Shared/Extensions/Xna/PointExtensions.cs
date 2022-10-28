namespace DaLion.Shared.Extensions.Xna;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="Point"/> struct.</summary>
internal static class PointExtensions
{
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
}

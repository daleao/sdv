namespace DaLion.Common.Extensions.Xna;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

internal static class PointExtensions
{
    /// <summary>Draw a border of specified height and width starting at the <see cref="Point"/> instance.</summary>
    /// <param name="height">The height of the border.</param>
    /// <param name="width">The width of the border.</param>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="b"><see cref="SpriteBatch" /> to draw to.</param>
    public static void DrawBorder(this Point p, int height, int width, Texture2D pixel, int thickness, Color color, SpriteBatch b)
    {
        var (x, y) = p;
        b.Draw(pixel, new Rectangle(x, y, width, thickness), color); // top line
        b.Draw(pixel, new Rectangle(x, y, thickness, height), color); // left line
        b.Draw(pixel, new Rectangle(x + width - thickness, y, thickness, height), color); // right line
        b.Draw(pixel, new Rectangle(x, y + height - thickness, width, thickness), color); // bottom line
    }

    /// <summary>Draw a border of specified height and width starting at the <see cref="Point"/> instance.</summary>
    /// <param name="height">The height of the border.</param>
    /// <param name="width">The width of the border.</param>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="b"><see cref="SpriteBatch" /> to draw to.</param>
    public static void DrawBorder(this Point p, int height, int width, Texture2D pixel, int thickness, Color color, SpriteBatch b, Vector2 offset)
    {
        var (x, y) = p + offset.ToPoint();
        b.Draw(pixel, new Rectangle(x, y, width, thickness), color); // top line
        b.Draw(pixel, new Rectangle(x, y, thickness, height), color); // left line
        b.Draw(pixel, new Rectangle(x + width - thickness, y, thickness, height), color); // right line
        b.Draw(pixel, new Rectangle(x, y + height - thickness, width, thickness), color); // bottom line
    }
}
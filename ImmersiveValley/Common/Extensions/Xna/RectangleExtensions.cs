namespace DaLion.Common.Extensions.Xna;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="Rectangle"/> class.</summary>
public static class RectangleExtensions
{
    /// <summary>Draw the rectangle's border to the specified <see cref="SpriteBatch"/>.</summary>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="b"><see cref="SpriteBatch" /> to draw to.</param>
    public static void DrawBorder(this Rectangle r, Texture2D pixel, int thickness, Color color, SpriteBatch b)
    {
        b.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, thickness), color); // top line
        b.Draw(pixel, new Rectangle(r.X, r.Y, thickness, r.Height), color); // left line
        b.Draw(pixel, new Rectangle(r.X + r.Width - thickness, r.Y, thickness, r.Height), color); // right line
        b.Draw(pixel, new Rectangle(r.X, r.Y + r.Height - thickness, r.Width, thickness), color); // bottom line
    }

    /// <summary>Draw the rectangle's border to the specified <see cref="SpriteBatch"/>.</summary>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="b"><see cref="SpriteBatch" /> to draw to.</param>
    public static void DrawBorder(this Rectangle r, Texture2D pixel, int thickness, Color color, SpriteBatch b,
        Vector2 offset)
    {
        b.Draw(pixel, new Rectangle(r.X + (int) offset.X, r.Y + (int) offset.Y, r.Width, thickness), color); // top line
        b.Draw(pixel, new Rectangle(r.X + (int) offset.X, r.Y + (int) offset.Y, thickness, r.Height), color); // left line
        b.Draw(pixel, new Rectangle(r.X + (int) offset.X + r.Width - thickness, r.Y + (int) offset.Y, thickness, r.Height), color); // right line
        b.Draw(pixel, new Rectangle(r.X + (int) offset.X, r.Y + (int) offset.Y + r.Height - thickness, r.Width, thickness), color); // bottom line
    }
}
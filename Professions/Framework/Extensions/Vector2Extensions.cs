namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Professions.Framework.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="Vector2"/> struct.</summary>
internal static class Vector2Extensions
{
    /// <summary>Draws a pointer over the <paramref name="tile"/> if it is inside the current viewport.</summary>
    /// <param name="tile">The <see cref="Vector2"/> tile.</param>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="color">The desired color for the pointer.</param>
    internal static void TrackWhenOnScreen(this Vector2 tile, SpriteBatch spriteBatch, Color color)
    {
        HudPointer.Instance.DrawOverTile(spriteBatch, tile, color);
    }

    /// <summary>
    ///     Draws a pointer at the edge of the screen, pointing to the <paramref name="tile"/>, if it is outside the
    ///     current viewport.
    /// </summary>
    /// <param name="tile">The <see cref="Vector2"/> tile.</param>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="color">The desired color for the pointer.</param>
    internal static void TrackWhenOffScreen(this Vector2 tile, SpriteBatch spriteBatch, Color color)
    {
        HudPointer.Instance.DrawAsTrackingPointer(spriteBatch, tile, color);
    }
}

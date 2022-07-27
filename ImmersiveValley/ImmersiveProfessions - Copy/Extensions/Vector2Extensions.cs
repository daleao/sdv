namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

public static class Vector2Extensions
{
    /// <summary>Draw a pointer over the tile if it is inside the current viewport.</summary>
    /// <param name="color">The desired color for the pointer.</param>
    public static void TrackWhenOnScreen(this Vector2 tile, Color color)
    {
        ModEntry.Player.Pointer.DrawOverTile(tile, color);
    }

    /// <summary>Draw a pointer at the edge of the screen, pointing to the tile, if it is outside the current viewport.</summary>
    /// <param name="color">The desired color for the pointer.</param>
    public static void TrackWhenOffScreen(this Vector2 tile, Color color)
    {
        ModEntry.Player.Pointer.DrawAsTrackingPointer(tile, color);
    }
}
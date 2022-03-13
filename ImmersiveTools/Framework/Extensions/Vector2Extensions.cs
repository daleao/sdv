namespace DaLion.Stardew.Tools.Framework.Extensions;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

public static class Vector2Extensions
{
    /// <summary>Get the pixel position relative to the top-left corner of the map.</summary>
    public static Vector2 GetPixelPosition(this Vector2 tilePosition)
    {
        return tilePosition * Game1.tileSize + new Vector2(Game1.tileSize / 2f);
    }

    /// <summary>Get a rectangle representing the tile area in absolute pixels from the map origin.</summary>
    public static Rectangle GetAbsoluteTileArea(this Vector2 tilePosition)
    {
        var (x, y) = tilePosition * Game1.tileSize;
        return new((int) x, (int) y, Game1.tileSize, Game1.tileSize);
    }
}
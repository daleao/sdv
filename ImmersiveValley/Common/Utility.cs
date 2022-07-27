namespace DaLion.Common;

#region using directives

using Microsoft.Xna.Framework;
using System;

#endregion using directives

public static class Utility
{

    /// <summary>Get the unit vector which points towards the cursor's current position relative to the local player's position.</summary>
    public static Vector2 GetRelativeCursorDirection()
    {
        var (x, y) = Game1.currentCursorTile - Game1.player.getTileLocation();
        if (Math.Abs(x) > Math.Abs(y))
            return Vector2.UnitX * (x < 0 ? -1f : 1f);
        
        return Vector2.UnitY * (y < 0 ? 1f : -1f);
    }
}
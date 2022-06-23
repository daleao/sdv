namespace DaLion.Stardew.Tools.Extensions;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

public static class ToolExtensions
{
    /// <summary>Use a tool on a tile.</summary>
    /// <param name="tile">The tile to affect.</param>
    /// <param name="location">The current location.</param>
    /// <param name="who">The current player.</param>
    /// <returns><see langword="true"> for convenience when implementing tools.</returns>
    public static bool UseOnTile(this Tool tool, Vector2 tile, GameLocation location, Farmer who)
    {
        // use tool on center of tile
        who.lastClick = tile.GetPixelPosition();
        tool.DoFunction(location, (int)who.lastClick.X, (int)who.lastClick.Y, 0, who);
        return true;
    }
}
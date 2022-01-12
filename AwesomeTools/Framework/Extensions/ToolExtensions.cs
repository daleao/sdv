using Microsoft.Xna.Framework;
using StardewValley;

namespace TheLion.Stardew.Tools.Framework.Extensions
{
    public static class ToolExtensions
    {
        /// <summary>Use a tool on a tile.</summary>
        /// <param name="tile">The tile to affect.</param>
        /// <param name="location">The current location.</param>
        /// <param name="who">The current player.</param>
        /// <returns>Returns <c>true</c> for convenience when implementing tools.</returns>
        public static bool UseOnTile(this Tool tool, Vector2 tile, GameLocation location, Farmer who)
        {
            // use tool on center of tile
            who.lastClick = tile.GetPixelPosition();
            tool.DoFunction(location, (int)who.lastClick.X, (int)who.lastClick.Y, 0, who);
            return true;
        }
    }
}

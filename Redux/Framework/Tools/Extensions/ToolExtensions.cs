namespace DaLion.Redux.Framework.Tools.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Tool"/> class.</summary>
internal static class ToolExtensions
{
    /// <summary>Uses the <paramref name="tool"/> on the given <paramref name="tile"/>.</summary>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <param name="tile">The tile to affect.</param>
    /// <param name="location">The current location.</param>
    /// <param name="who">The current player.</param>
    /// <returns>Always <see langword="true"/>, for convenience when implementing tools.</returns>
    internal static bool UseOnTile(this Tool tool, Vector2 tile, GameLocation location, Farmer who)
    {
        // use tool on center of tile
        who.lastClick = tile.GetPixelPosition();
        tool.DoFunction(location, (int)who.lastClick.X, (int)who.lastClick.Y, 0, who);
        return true;
    }
}

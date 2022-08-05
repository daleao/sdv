namespace DaLion.Stardew.Arsenal.Extensions;

#region using directives

using Common.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

#endregion using directives

/// <summary>Extensions for the <see cref="GameLocation"/> class.</summary>
public static class GameLocationExtensions
{
    /// <summary>Whether the specified tile is likely to contain a snowy texture.</summary>
    public static bool DoesTileHaveSnow(this GameLocation location, Vector2 tile)
    {
        if (location is IslandLocation || !location.IsOutdoors || location.GetSeasonForLocation() != "winter") return false;

        var (x, y) = tile;
        return location.doesTileHaveProperty((int)x, (int)y, "Type", "Back")?.IsIn("Dirt", "Grass") == true;
    }
}
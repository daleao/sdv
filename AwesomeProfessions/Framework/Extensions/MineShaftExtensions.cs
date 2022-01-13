using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using DaLion.Stardew.Common.Extensions;

namespace DaLion.Stardew.Professions.Framework.Extensions;

public static class MineShaftExtensions
{
    /// <summary>Whether the current mine level is a safe level; i.e. shouldn't spawn any monsters.</summary>
    /// <param name="shaft">The <see cref="MineShaft" /> instance.</param>
    public static bool IsTreasureOrSafeRoom(this MineShaft shaft)
    {
        return shaft.mineLevel <= 120 && shaft.mineLevel % 10 == 0 ||
               shaft.mineLevel == 220 && Game1.player.secretNotesSeen.Contains(10) &&
               !Game1.player.mailReceived.Contains("qiCave") || ModEntry.ModHelper.Reflection
                   .GetField<NetBool>(shaft, "netIsTreasureRoom").GetValue().Value;
    }

    /// <summary>Find all tiles in a mine map containing either a ladder or shaft.</summary>
    /// <param name="shaft">The MineShaft location.</param>
    /// <remarks>Credit to <c>pomepome</c>.</remarks>
    public static IEnumerable<Vector2> GetLadderTiles(this MineShaft shaft)
    {
        for (var i = 0; i < shaft.Map.GetLayer("Buildings").LayerWidth; ++i)
        for (var j = 0; j < shaft.Map.GetLayer("Buildings").LayerHeight; ++j)
        {
            var index = shaft.getTileIndexAt(new(i, j), "Buildings");
            if (index.IsAnyOf(173, 174)) yield return new(i, j);
        }
    }
}
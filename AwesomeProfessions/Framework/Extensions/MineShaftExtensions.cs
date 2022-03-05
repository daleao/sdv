namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;

using Common.Extensions;

#endregion using directives

internal static class MineShaftExtensions
{
    private static readonly FieldInfo _NetIsTreasureRoom = typeof(MineShaft).Field("netIsTreasureRoom");

    /// <summary>Whether the current mine level is a safe level; i.e. shouldn't spawn any monsters.</summary>
    /// <param name="shaft">The <see cref="MineShaft" /> instance.</param>
    internal static bool IsTreasureOrSafeRoom(this MineShaft shaft)
    {
        return shaft.mineLevel <= 120 && shaft.mineLevel % 10 == 0 ||
               shaft.mineLevel == 220 && Game1.player.secretNotesSeen.Contains(10) &&
               !Game1.player.mailReceived.Contains("qiCave") || ((NetBool) _NetIsTreasureRoom.GetValue(shaft))!.Value;
    }

    /// <summary>Find all tiles in a mine map containing either a ladder or shaft.</summary>
    /// <param name="shaft">The MineShaft location.</param>
    /// <remarks>Credit to <c>pomepome</c>.</remarks>
    internal static IEnumerable<Vector2> GetLadderTiles(this MineShaft shaft)
    {
        for (var i = 0; i < shaft.Map.GetLayer("Buildings").LayerWidth; ++i)
        for (var j = 0; j < shaft.Map.GetLayer("Buildings").LayerHeight; ++j)
        {
            var index = shaft.getTileIndexAt(new(i, j), "Buildings");
            if (index.IsAnyOf(173, 174)) yield return new(i, j);
        }
    }
}
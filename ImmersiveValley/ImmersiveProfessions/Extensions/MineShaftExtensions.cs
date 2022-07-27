namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common.Extensions;
using Common.Extensions.Reflection;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

#endregion using directives

/// <summary>Extensions for the <see cref="MineShaft"/> class.</summary>
public static class MineShaftExtensions
{
    private static Func<MineShaft, NetBool>? _GetNetIsTreasureRoom;

    /// <summary>Whether the current mine level is a safe level; i.e. shouldn't spawn any monsters.</summary>
    /// <param name="shaft">The <see cref="MineShaft" /> instance.</param>
    public static bool IsTreasureOrSafeRoom(this MineShaft shaft)
    {
        _GetNetIsTreasureRoom ??= typeof(MineShaft).RequireField("netIsTreasureRoom")
            .CompileUnboundFieldGetterDelegate<MineShaft, NetBool>();
        return shaft.mineLevel <= 120 && shaft.mineLevel % 10 == 0 ||
               shaft.mineLevel == 220 && Game1.player.secretNotesSeen.Contains(10) &&
               !Game1.player.mailReceived.Contains("qiCave") || _GetNetIsTreasureRoom(shaft).Value;
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
                if (index.IsIn(173, 174)) yield return new(i, j);
            }
    }
}
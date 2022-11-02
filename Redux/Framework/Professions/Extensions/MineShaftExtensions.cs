namespace DaLion.Redux.Framework.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;

#endregion using directives

/// <summary>Extensions for the <see cref="MineShaft"/> class.</summary>
internal static class MineShaftExtensions
{
    /// <summary>Determines whether the current mine level is a safe level; i.e. shouldn't spawn any monsters.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/> instance.</param>
    /// <returns><see langword="true"/> if the <paramref name="shaft"/>'s level is a regular mine level multiple of 10 or a skull cavern level with either a Qi event or the treasure net flag, otherwise <see langword="false"/>.</returns>
    internal static bool IsTreasureOrSafeRoom(this MineShaft shaft)
    {
        var isTreasureRoom = ModEntry.Reflector
            .GetUnboundFieldGetter<MineShaft, NetBool>(shaft, "netIsTreasureRoom")
            .Invoke(shaft).Value;
        return (shaft.mineLevel <= 120 && shaft.mineLevel % 10 == 0) ||
               (shaft.mineLevel == 220 && Game1.player.secretNotesSeen.Contains(10) &&
                !Game1.player.mailReceived.Contains("qiCave")) || isTreasureRoom;
    }

    /// <summary>Finds all tiles in a mine map containing either a ladder or sink-hole.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/> instance.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all the <see cref="Vector2"/> tiles that contain a ladder or sink-hole.</returns>
    /// <remarks>Credit to <c>pomepome</c>.</remarks>
    internal static IEnumerable<Vector2> GetLadderTiles(this MineShaft shaft)
    {
        for (var i = 0; i < shaft.Map.GetLayer("Buildings").LayerWidth; ++i)
        {
            for (var j = 0; j < shaft.Map.GetLayer("Buildings").LayerHeight; ++j)
            {
                var index = shaft.getTileIndexAt(new Point(i, j), "Buildings");
                if (index.IsIn(173, 174))
                {
                    yield return new Vector2(i, j);
                }
            }
        }
    }
}

namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(Tool), "tilesAffected")]
internal sealed class ToolTilesAffectedPatcher
{
    private static uint[] AxeAffectedTilesRadii => Config.Axe.RadiusAtEachPowerLevel;

    private static uint[] PickaxeAffectedTilesRadii => Config.Pick.RadiusAtEachPowerLevel;

    #region harmony patches

    /// <summary>Override affected tiles for resource tools.</summary>
    [HarmonyPostfix]
    private static void Postfix(
        Tool __instance, List<Vector2> __result, Vector2 tileLocation, int power)
    {
        if (__instance.UpgradeLevel < Tool.copper || __instance is not (Axe or Pickaxe))
        {
            return;
        }

        __result.Clear();
        var radius = __instance is Axe
            ? AxeAffectedTilesRadii[Math.Min(power - 2, 4)]
            : PickaxeAffectedTilesRadii[Math.Min(power - 2, 4)];
        if (radius == 0)
        {
            return;
        }

        var circle = new CircleTileGrid(tileLocation, radius);
        __result.AddRange(circle.Tiles);
    }

    #endregion harmony patches
}

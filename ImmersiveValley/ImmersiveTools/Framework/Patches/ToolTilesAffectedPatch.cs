namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

using Common.Classes;
using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolTilesAffectedPatch : BasePatch
{
    private static int[] AxeAffectedTilesRadii => ModEntry.Config.AxeConfig.RadiusAtEachPowerLevel;
    private static int[] PickaxeAffectedTilesRadii => ModEntry.Config.PickaxeConfig.RadiusAtEachPowerLevel;
    private static int[][] HoeAffectedTiles => ModEntry.Config.HoeConfig.AffectedTiles;
    private static int[][] WateringCanAffectedTiles => ModEntry.Config.WateringCanConfig.AffectedTiles;

    /// <summary>Construct an instance.</summary>
    internal ToolTilesAffectedPatch()
    {
        Target = RequireMethod<Tool>("tilesAffected");
        Prefix!.priority = Priority.HigherThanNormal;
        Postfix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Override affected tiles for farming tools.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool ToolTilesAffectedPrefix(Tool __instance, ref List<Vector2> __result, Vector2 tileLocation, ref int power, Farmer who)
    {
        if (__instance is not (Hoe or WateringCan) || power < 1) return true; // run original logic

        if (__instance is Hoe && !ModEntry.Config.HoeConfig.OverrideAffectedTiles || __instance is WateringCan &&
            !ModEntry.Config.WateringCanConfig.OverrideAffectedTiles)
            return true; // run original logic

        var len = __instance is Hoe ? HoeAffectedTiles[power - 1][0] : WateringCanAffectedTiles[power - 1][0];
        var rad = __instance is Hoe ? HoeAffectedTiles[power - 1][1] : WateringCanAffectedTiles[power - 1][1];

        __result = new();
        var dir = who.FacingDirection switch
        {
            0 => new(0f, -1f),
            1 => new(1f, 0f),
            2 => new(0f, 1f),
            3 => new(-1f, 0f),
            _ => Vector2.Zero
        };

        var perp = new Vector2(dir.Y, dir.X);
        for (var il = 0; il < len; il++)
            for (var ir = -rad; ir <= rad; ir++)
                __result.Add(tileLocation + dir * il + perp * ir);

        ++power;
        return false; // don't run original logic
    }

    /// <summary>Override affected tiles for resource tools.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static void ToolTilesAffectedPostfix(Tool __instance, List<Vector2> __result, Vector2 tileLocation, int power)
    {
        if (__instance.UpgradeLevel < Tool.copper || __instance is not (Axe or Pickaxe))
            return;

        __result.Clear();
        var radius = __instance is Axe
            ? AxeAffectedTilesRadii[Math.Min(power - 2, 4)]
            : PickaxeAffectedTilesRadii[Math.Min(power - 2, 4)];
        if (radius == 0)
            return;

        var circle = new CircleTileGrid(tileLocation, radius);
        __result.AddRange(circle.Tiles);
    }

    #endregion harmony patches
}
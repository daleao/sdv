namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Classes;
using DaLion.Shared.Extensions.Xna;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolTilesAffectedPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ToolTilesAffectedPatch"/> class.</summary>
    internal ToolTilesAffectedPatch()
    {
        this.Target = this.RequireMethod<Tool>("tilesAffected");
        this.Prefix!.priority = Priority.HigherThanNormal;
        this.Postfix!.priority = Priority.LowerThanNormal;
    }

    private static int[] AxeAffectedTilesRadii => ModEntry.Config.Tools.Axe.RadiusAtEachPowerLevel;

    private static int[] PickaxeAffectedTilesRadii => ModEntry.Config.Tools.Pick.RadiusAtEachPowerLevel;

    private static int[][] HoeAffectedTiles => ModEntry.Config.Tools.Hoe.AffectedTiles;

    private static int[][] WateringCanAffectedTiles => ModEntry.Config.Tools.Can.AffectedTiles;

    #region harmony patches

    /// <summary>Override affected tiles for farming tools.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool ToolTilesAffectedPrefix(
        Tool __instance, ref List<Vector2> __result, Vector2 tileLocation, ref int power, Farmer who)
    {
        if (__instance is not (Hoe or WateringCan) || power < 1)
        {
            return true; // run original logic
        }

        if ((__instance is Hoe && !ModEntry.Config.Tools.Hoe.OverrideAffectedTiles) || (__instance is WateringCan &&
                !ModEntry.Config.Tools.Can.OverrideAffectedTiles))
        {
            return true; // run original logic
        }

        var length = __instance is Hoe ? HoeAffectedTiles[power - 1][0] : WateringCanAffectedTiles[power - 1][0];
        var radius = __instance is Hoe ? HoeAffectedTiles[power - 1][1] : WateringCanAffectedTiles[power - 1][1];

        __result = new List<Vector2>();
        var direction = who.FacingDirection switch
        {
            Game1.up => new Vector2(0f, -1f),
            Game1.right => new Vector2(1f, 0f),
            Game1.down => new Vector2(0f, 1f),
            Game1.left => new Vector2(-1f, 0f),
            _ => Vector2.Zero,
        };

        var perpendicular = direction.Perpendicular();
        for (var l = 0; l < length; ++l)
        {
            for (var r = -radius; r <= radius; ++r)
            {
                __result.Add(tileLocation + (direction * l) + (perpendicular * r));
            }
        }

        ++power;
        return false; // don't run original logic
    }

    /// <summary>Override affected tiles for resource tools.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static void ToolTilesAffectedPostfix(
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

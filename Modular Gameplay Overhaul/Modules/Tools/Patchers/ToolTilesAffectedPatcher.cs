namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Classes;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolTilesAffectedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolTilesAffectedPatcher"/> class.</summary>
    internal ToolTilesAffectedPatcher()
    {
        this.Target = this.RequireMethod<Tool>("tilesAffected");
        this.Prefix!.priority = Priority.HigherThanNormal;
        this.Postfix!.priority = Priority.LowerThanNormal;
    }

    private static uint[] AxeAffectedTilesRadii => ToolsModule.Config.Axe.RadiusAtEachPowerLevel;

    private static uint[] PickaxeAffectedTilesRadii => ToolsModule.Config.Pick.RadiusAtEachPowerLevel;

    private static (uint Length, uint Radius)[] HoeAffectedTiles => ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel;

    private static (uint Length, uint Radius)[] WateringCanAffectedTiles => ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel;

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

        if ((__instance is Hoe && !ToolsModule.Config.Hoe.OverrideAffectedTiles) || (__instance is WateringCan &&
                !ToolsModule.Config.Can.OverrideAffectedTiles))
        {
            return true; // run original logic
        }

        var length = __instance is Hoe ? HoeAffectedTiles[power - 1].Length : WateringCanAffectedTiles[power - 1].Length;
        var radius = __instance is Hoe ? HoeAffectedTiles[power - 1].Radius : WateringCanAffectedTiles[power - 1].Radius;

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
        for (var l = 0; l < length; l++)
        {
            for (var r = -radius; r <= radius; r++)
            {
                __result.Add(tileLocation + (direction * l) + (perpendicular * r));
            }
        }

        power++;
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

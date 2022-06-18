namespace DaLion.Stardew.Tools.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

using Common.Classes;

#endregion using directives

/// <summary>Patches the game code to implement modded tool behavior.</summary>
[UsedImplicitly]
internal static class Patches
{
    private static int[] AxeAffectedTilesRadii => ModEntry.Config.AxeConfig.RadiusAtEachPowerLevel;
    private static int[] PickaxeAffectedTilesRadii => ModEntry.Config.PickaxeConfig.RadiusAtEachPowerLevel;
    private static int[][] HoeAffectedTiles => ModEntry.Config.HoeConfig.AffectedTiles;
    private static int[][] WateringCanAffectedTiles => ModEntry.Config.WateringCanConfig.AffectedTiles;

    #region harmony patches

    [HarmonyPatch(typeof(Farmer), "toolPowerIncrease")]
    internal sealed class FarmerToolPowerIncreasePatch
    {
        /// <summary>Allow first two power levels on Pickaxe.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var l = instructions.ToList();
            for (var i = 0; i < l.Count; ++i)
            {
                if (l[i].opcode != OpCodes.Isinst ||
                    l[i].operand?.ToString() != "StardewValley.Tools.Pickaxe") continue;

                // inject branch over toolPower += 2
                l.Insert(i - 2, new(OpCodes.Br_S, l[i + 1].operand));
                break;
            }

            return l.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(Axe), "beginUsing")]
    internal sealed class AxeBeginUsingPatch
    {
        /// <summary>Enable Axe power level increase.</summary>
        [HarmonyPrefix]
        private static bool Prefix(Tool __instance, Farmer who)
        {
            if (!ModEntry.Config.AxeConfig.EnableCharging ||
                ModEntry.Config.RequireModkey && !ModEntry.Config.Modkey.IsDown() ||
                __instance.UpgradeLevel < (int)ModEntry.Config.AxeConfig.RequiredUpgradeForCharging)
                return true; // run original logic

            who.Halt();
            __instance.Update(who.FacingDirection, 0, who);
            switch (who.FacingDirection)
            {
                case 0:
                    who.FarmerSprite.setCurrentFrame(176);
                    __instance.Update(0, 0, who);
                    break;

                case 1:
                    who.FarmerSprite.setCurrentFrame(168);
                    __instance.Update(1, 0, who);
                    break;

                case 2:
                    who.FarmerSprite.setCurrentFrame(160);
                    __instance.Update(2, 0, who);
                    break;

                case 3:
                    who.FarmerSprite.setCurrentFrame(184);
                    __instance.Update(3, 0, who);
                    break;
            }

            return false; // don't run original logic
        }
    }

    [HarmonyPatch(typeof(Pickaxe), "beginUsing")]
    internal sealed class PickaxeBeginUsingPatch
    {
        /// <summary>Enable Pickaxe power level increase.</summary>
        [HarmonyPrefix]
        private static bool Prefix(Tool __instance, Farmer who)
        {
            if (!ModEntry.Config.PickaxeConfig.EnableCharging ||
                ModEntry.Config.RequireModkey && !ModEntry.Config.Modkey.IsDown() ||
                __instance.UpgradeLevel < (int)ModEntry.Config.PickaxeConfig.RequiredUpgradeForCharging)
                return true; // run original logic

            who.Halt();
            __instance.Update(who.FacingDirection, 0, who);
            switch (who.FacingDirection)
            {
                case 0: // up
                    who.FarmerSprite.setCurrentFrame(176);
                    __instance.Update(0, 0, who);
                    break;

                case 1: // right
                    who.FarmerSprite.setCurrentFrame(168);
                    __instance.Update(1, 0, who);
                    break;

                case 2: // down
                    who.FarmerSprite.setCurrentFrame(160);
                    __instance.Update(2, 0, who);
                    break;

                case 3: // left
                    who.FarmerSprite.setCurrentFrame(184);
                    __instance.Update(3, 0, who);
                    break;
            }

            return false; // don't run original logic
        }
    }

    [HarmonyPatch(typeof(Tool), nameof(Tool.endUsing))]
    internal sealed class ToolEndUsingPatch
    {
        /// <summary>Do shockwave.</summary>
        [HarmonyPostfix]
        private static void Postfix(Farmer who)
        {
            var tool = who.CurrentTool;
            if (who.toolPower <= 0 || tool is not (Axe or Pickaxe)) return;

            var radius = 1;
            var power = who.toolPower;
            switch (tool)
            {
                case Axe:
                    who.Stamina -=
                        (float) Math.Pow(ModEntry.Config.StaminaCostMultiplier * power - who.ForagingLevel * 0.1f, 2f);
                    radius = ModEntry.Config.AxeConfig.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1);
                    break;

                case Pickaxe:
                    who.Stamina -=
                        (float) Math.Pow(ModEntry.Config.StaminaCostMultiplier * power - who.MiningLevel * 0.1f, 2f);
                    radius = ModEntry.Config.PickaxeConfig.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1);
                    break;
            }

            ModEntry.Shockwave.Value = new(radius, who, Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
        }
    }

    [HarmonyPatch(typeof(Tool), "tilesAffected")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    internal sealed class ToolTilesAffectedPatch
    {
        /// <summary>Override affected tiles for farming tools.</summary>
        [HarmonyPrefix]
        private static bool Prefix(Tool __instance, ref List<Vector2> __result, Vector2 tileLocation, ref int power, Farmer who)
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
        private static void Postfix(Tool __instance, List<Vector2> __result, Vector2 tileLocation, int power)
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
    }

    [HarmonyPatch(typeof(Tool), "draw")]
    internal sealed class ToolDrawPatch
    {
        /// <summary>Hide affected tiles overlay for Axe or Pickaxe.</summary>
        [HarmonyPrefix]
        private static bool Prefix(Tool __instance)
        {
            return !ModEntry.Config.HideAffectedTiles;
        }
    }

    [HarmonyPatch(typeof(ReachingToolEnchantment), nameof(ReachingToolEnchantment.CanApplyTo))]
    internal sealed class ReachingToolEnchantmentCanApplyToPatch
    {
        /// <summary>Allow apply reaching enchant.</summary>
        [HarmonyPrefix]
        // ReSharper disable once RedundantAssignment
        private static bool Prefix(ref bool __result, Item item)
        {
            if (item is Tool tool && (tool is WateringCan or Hoe ||
                                      tool is Axe && ModEntry.Config.AxeConfig.AllowReachingEnchantment ||
                                      tool is Pickaxe && ModEntry.Config.PickaxeConfig.AllowReachingEnchantment))
                __result = tool.UpgradeLevel == 4;
            else
                __result = false;

            return false; // don't run original logic
        }
    }

    #endregion harmony patches
}
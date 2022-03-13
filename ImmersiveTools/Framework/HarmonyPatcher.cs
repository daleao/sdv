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
using Common.Extensions;

#endregion using directives

/// <summary>Patches the game code to implement modded tool behavior.</summary>
[UsedImplicitly]
internal static class HarmonyPatcher
{
    private static int[] AxeAffectedTilesRadii => ModEntry.Config.AxeConfig.RadiusAtEachPowerLevel;
    private static int[] PickaxeAffectedTilesRadii => ModEntry.Config.PickaxeConfig.RadiusAtEachPowerLevel;
    private static int[][] HoeAffectedTiles => ModEntry.Config.HoeConfig.AffectedTiles;
    private static int[][] WateringCanAffectedTiles => ModEntry.Config.WateringCanConfig.AffectedTiles;

    #region harmony patches

    // allow first two power levels on Pickaxe
    [HarmonyPatch(typeof(Farmer), "toolPowerIncrease")]
    internal class FarmerToolPowerIncreasePatch
    {
        [HarmonyTranspiler]
        protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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

    // enable Axe power level increase
    [HarmonyPatch(typeof(Axe), "beginUsing")]
    internal class AxeBeginUsingPatch
    {
        [HarmonyPrefix]
        protected static bool Prefix(Tool __instance, Farmer who)
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

    // enable Pickaxe power level increase
    [HarmonyPatch(typeof(Pickaxe), "beginUsing")]
    internal class PickaxeBeginUsingPatch
    {
        [HarmonyPrefix]
        protected static bool Prefix(Tool __instance, Farmer who)
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

    // do shockwave
    [HarmonyPatch(typeof(Tool), nameof(Tool.endUsing))]
    internal class ToolEndUsingPatch
    {
        [HarmonyPostfix]
        protected static void Postfix(Farmer who)
        {
            var tool = who.CurrentTool;
            if (who.toolPower <= 0 || tool is not (Axe or Pickaxe)) return;

            var radius = 1;
            switch (tool)
            {
                case Axe:
                    who.Stamina -= who.toolPower - who.ForagingLevel * 0.1f * (who.toolPower - 1) *
                        ModEntry.Config.StaminaCostMultiplier;
                    radius = ModEntry.Config.AxeConfig.RadiusAtEachPowerLevel.ElementAtOrDefault(who.toolPower - 1);
                    break;

                case Pickaxe:
                    who.Stamina -= who.toolPower - who.MiningLevel * 0.1f * (who.toolPower - 1) *
                        ModEntry.Config.StaminaCostMultiplier;
                    radius = ModEntry.Config.PickaxeConfig.RadiusAtEachPowerLevel.ElementAtOrDefault(who.toolPower - 1);
                    break;
            }

            ModEntry.Shockwave.Value = new(radius, who, Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
        }
    }

    // override affected tiles for all tools
    [HarmonyPatch(typeof(Tool), "tilesAffected")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    internal class ToolTilesAffectedPatch
    {
        [HarmonyPrefix]
        protected static bool Prefix(Tool __instance, ref List<Vector2> __result, Vector2 tileLocation, ref int power, Farmer who)
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

        [HarmonyPostfix]
        protected static void Postfix(Tool __instance, List<Vector2> __result, Vector2 tileLocation, int power)
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

    // hide affected tiles overlay for Axe or Pickaxe
    [HarmonyPatch(typeof(Tool), "draw")]
    internal class ToolDrawPatch
    {
        [HarmonyPrefix]
        protected static bool Prefix(Tool __instance)
        {
            return !ModEntry.Config.HideAffectedTiles;
        }
    }

    // allow apply reaching enchant
    [HarmonyPatch(typeof(ReachingToolEnchantment), nameof(ReachingToolEnchantment.CanApplyTo))]
    internal class ReachingToolEnchantmentCanApplyToPatch
    {
        [HarmonyPrefix]
        // ReSharper disable once RedundantAssignment
        protected static bool Prefix(ref bool __result, Item item)
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

    // allow apply magic/sunburst enchant
    [HarmonyPatch(typeof(BaseEnchantment), nameof(BaseEnchantment.GetAvailableEnchantments))]
    internal class BaseEnchantmentGetAvailableEnchantmentsPatch
    {
        [HarmonyTranspiler]
        protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var l = instructions.ToList();
            l.InsertRange(l.Count - 2, new List<CodeInstruction>
            {
                new(OpCodes.Ldsfld, typeof(BaseEnchantment).Field("_enchantments")),
                new(OpCodes.Newobj, typeof(MagicEnchantment).Constructor()),
                new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).MethodNamed(nameof(List<BaseEnchantment>.Add)))
            });

            return l.AsEnumerable();
        }
    }

    #endregion harmony patches
}
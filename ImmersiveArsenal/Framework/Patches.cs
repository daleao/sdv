namespace DaLion.Stardew.Arsenal.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

using Common.Extensions;
using Common.Harmony;

#endregion using directives

/// <summary>Patches the game code to implement modded arsenal behavior.</summary>
[UsedImplicitly]
internal static class Patches
{
    #region harmony patches

    [HarmonyPatch(typeof(Monster), nameof(Monster.parried))]
    internal class MonsterParriedPatch
    {
        /// <summary>Adds stamina cost to sword parry.</summary>
        [HarmonyPostfix]
        private static void MonsterParriedPostfix(Farmer who)
        {
            if (ModEntry.Config.WeaponsCostStamina)
                who.Stamina -= 2 - who.CombatLevel * 0.1f;
        }
    }

    [HarmonyPatch(typeof(JadeEnchantment), "_ApplyTo")]
    internal class JadeEnchantmentApplyToPatch
    {
        /// <summary>Rebalances Jade enchant.</summary>
        [HarmonyPostfix]
        private static void Postfix(JadeEnchantment __instance, Item item)
        {
            if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalanceEnchants) return;

            weapon.critMultiplier.Value += 0.4f * __instance.GetLevel();
        }
    }

    [HarmonyPatch(typeof(JadeEnchantment), "_UnapplyTo")]
    internal class JadeEnchantmentUnpplyToPatch
    {
        /// <summary>Rebalances Jade enchant.</summary>
        [HarmonyPostfix]
        private static void Postfix(JadeEnchantment __instance, Item item)
        {
            if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalanceEnchants) return;

            weapon.critMultiplier.Value -= 0.4f * __instance.GetLevel();
        }
    }

    [HarmonyPatch(typeof(TopazEnchantment), "_ApplyTo")]
    internal class TopazEnchantmentApplyToPatch
    {
        /// <summary>Rebalances Topaz enchant.</summary>
        [HarmonyPostfix]
        private static void Postfix(JadeEnchantment __instance, Item item)
        {
            if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalanceEnchants) return;

            weapon.addedDefense.Value += 4 * __instance.GetLevel();
        }
    }

    [HarmonyPatch(typeof(TopazEnchantment), "_UnapplyTo")]
    internal class TopazEnchantmentUnpplyToPatch
    {
        /// <summary>Rebalances Topaz enchant.</summary>
        [HarmonyPostfix]
        private static void Postfix(JadeEnchantment __instance, Item item)
        {
            if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalanceEnchants) return;

            weapon.addedDefense.Value -= 4 * __instance.GetLevel();
        }
    }

    [HarmonyPatch(typeof(BasicProjectile), MethodType.Constructor, typeof(int), typeof(int), typeof(int), typeof(int),
        typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(string), typeof(string), typeof(bool),
        typeof(bool), typeof(GameLocation), typeof(Character), typeof(bool),
        typeof(BasicProjectile.onCollisionBehavior))]
    internal class BasicProjectileCtorPatch
    {
        /// <summary>Removes slingshot grace period.</summary>
        [HarmonyPostfix]
        private static void Postfix(BasicProjectile __instance, bool damagesMonsters, Character firer)
        {
            if (damagesMonsters && firer is Farmer && ModEntry.Config.RemoveSlingshotGracePeriod)
                __instance.ignoreTravelGracePeriod.Value = true;
        }
    }

    [HarmonyPatch(typeof(Farmer), nameof(Farmer.takeDamage))]
    internal class FarmerTakeDamagePatch
    {
        /// <summary>Removes damage mitigation soft cap.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> FarmerTakeDamageTranspiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// Injected: if (ModEntry.Config.RemoveDefenseSoftCap)
            ///     skip
            ///     {
            ///         effectiveResilience >= damage * 0.5f)
            ///         effectiveResilience -= (int) (effectiveResilience * Game1.random.Next(3) / 10f);
            ///     }

            var skipSoftCap = generator.DefineLabel();
            try
            {
                helper
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldloc_3),
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.5f)
                    )
                    .StripLabels(out var labels)
                    .InsertWithLabels(
                        labels,
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).PropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(OpCodes.Call, typeof(ModConfig).PropertyGetter(nameof(ModConfig.RemoveDefenseSoftCap))),
                        new CodeInstruction(OpCodes.Brtrue_S, skipSoftCap)
                    )
                    .AdvanceUntil(
                        new CodeInstruction(OpCodes.Stloc_3)
                    )
                    .Advance()
                    .AddLabels(skipSoftCap);
            }
            catch (Exception ex)
            {
                Log.E($"Failed while removing vanilla defense cap.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.performTouchAction))]
    internal class GameLocationPerformTouchActionPatch
    {
        /// <summary>Apply new galaxy sword conditions.</summary>
        [HarmonyTranspiler]
        protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// From: if (Game1.player.ActiveObject != null && Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 74) && !Game1.player.mailReceived.Contains("galaxySword"))
            /// To: if (NewGalaxySwordConditions())

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldstr, "galaxySword")
                    )
                    .RetreatUntil(
                        new CodeInstruction(OpCodes.Brfalse)
                    )
                    .GetOperand(out var resumeExecution)
                    .RetreatUntil(
                        new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(Farmer).PropertyGetter(nameof(Farmer.ActiveObject))),
                        new CodeInstruction(OpCodes.Brfalse)
                    )
                    .GetLabels(out var labels)
                    .RemoveUntil(
                        new CodeInstruction(OpCodes.Brtrue)
                    )
                    .InsertWithLabels(
                        labels,
                        new CodeInstruction(OpCodes.Call,
                            typeof(Patches).MethodNamed(nameof(NewGalaxySwordConditions))),
                        new CodeInstruction(OpCodes.Brfalse, resumeExecution)
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed injecting custom legendary sword conditions.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(BaseEnchantment), nameof(BaseEnchantment.GetAvailableEnchantments))]
    internal class BaseEnchantmentGetAvailableEnchantmentsPatch
    {
        /// <summary>Allow applying magic/sunburst enchant.</summary>
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

    #region private methods

    private static bool NewGalaxySwordConditions()
    {
        return Game1.player.ActiveObject != null &&
               Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 896) &&
               !Game1.player.mailReceived.Contains("galaxySword");
    }

    #endregion private methods
}
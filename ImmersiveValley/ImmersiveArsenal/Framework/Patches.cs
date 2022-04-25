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
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.Tools;

using Common.Harmony;
using Common.Extensions.Reflection;

using SObject = StardewValley.Object;

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
            if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalancedEnchants) return;

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
            if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalancedEnchants) return;

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
            if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalancedEnchants) return;

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
            if (item is not MeleeWeapon weapon || !ModEntry.Config.RebalancedEnchants) return;

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
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(OpCodes.Call, typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.RemoveDefenseSoftCap))),
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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// From: Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 74)
            /// To: Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, ModEntry.Config.TrulyLegendaryGalaxySword ? Constants.GALAXY_SOUL_INDEX_I : 74)
            ///     -- and also
            /// Injected: this.playSound("thunder");

            var trulyLegendaryGalaxySword = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldc_I4_S, 74)
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(OpCodes.Call,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.TrulyLegendaryGalaxySword))),
                        new CodeInstruction(OpCodes.Brtrue_S, trulyLegendaryGalaxySword)
                    )
                    .Advance()
                    .AddLabels(resumeExecution)
                    .Insert(
                        new CodeInstruction(OpCodes.Br_S, resumeExecution)
                    )
                    .InsertWithLabels(
                        new[] {trulyLegendaryGalaxySword},
                        new CodeInstruction(OpCodes.Ldc_I4, Constants.GALAXY_SOUL_INDEX_I)
                    )
                    .AdvanceUntil(
                        new CodeInstruction(OpCodes.Brtrue)
                    )
                    .Advance()
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldstr, "thunder"),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Call,
                            typeof(GameLocation).RequireMethod(nameof(GameLocation.playSound)))
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

    [HarmonyPatch(typeof(Game1), nameof(Game1.applySaveFix))]
    internal class Game1ApplySaveFixPatch
    {
        /// <summary>Replace with custom Qi Challenge.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// From: if (farmer.mailReceived.Contains("skullCave") && !farmer.hasQuest(20) && !farmer.hasOrWillReceiveMail("QiChallengeComplete"))
            /// To: if (farmer.mailReceived.Contains("skullCave")) CheckForMissingQiChallenges(farmer)

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldstr, "skullCave")
                    )
                    .AdvanceUntil(
                        new CodeInstruction(OpCodes.Ldloc_S)
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[23]),
                        new CodeInstruction(OpCodes.Call,
                            typeof(Game1ApplySaveFixPatch).RequireMethod(nameof(CheckForMissingQiChallenges)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed adding failsafe for custom Qi Challenges.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }

        private static void CheckForMissingQiChallenges(Farmer farmer)
        {
            if (farmer.hasOrWillReceiveMail("QiChallengeComplete")) return;

            if (ModEntry.Config.TrulyLegendaryGalaxySword)
            {
                if (!farmer.hasQuest(20) && !farmer.hasOrWillReceiveMail("QiChallengeFirst"))
                    farmer.addQuest(20);
                else if (farmer.hasOrWillReceiveMail("QiChallengeFirst") &&
                         !farmer.hasQuest(ModEntry.QiChallengeFinalQuestId))
                    farmer.addQuest(ModEntry.QiChallengeFinalQuestId);
            }
            else
            {
                if (!farmer.hasQuest(20)) farmer.addQuest(20);
            }
        }
    }

    [HarmonyPatch(typeof(MineShaft), nameof(MineShaft.CheckForQiChallengeCompletion))]
    internal class MineShaftCheckForQiChallengeCompletionPatch
    {
        /// <summary>Add custom quest.</summary>
        [HarmonyPostfix]
        private static bool Prefix()
        {
            if (!ModEntry.Config.TrulyLegendaryGalaxySword) return true; // run original logic
            
            if (Game1.player.deepestMineLevel >= 145 && Game1.player.hasQuest(20) && !Game1.player.hasOrWillReceiveMail("QiChallengeFirst"))
            {
                Game1.player.completeQuest(20);
                Game1.addMailForTomorrow("QiChallengeFirst");
            }
            else if (Game1.player.deepestMineLevel >= 170 && Game1.player.hasQuest(ModEntry.QiChallengeFinalQuestId) &&
                     !Game1.player.hasOrWillReceiveMail("QiChallengeComplete"))
            {
                Game1.player.completeQuest(ModEntry.QiChallengeFinalQuestId);
                Game1.addMailForTomorrow("QiChallengeComplete");
            }

            return false; // don't run original logic
        }
    }

    [HarmonyPatch(typeof(MineShaft), nameof(MineShaft.loadLevel))]
    internal class MineShaftLoadLevelPatch
    {
        /// <summary>Create Qi Challenge reward level.</summary>
        [HarmonyPostfix]
        private static bool Prefix(MineShaft __instance, ref NetBool ___netIsTreasureRoom, int level)
        {
            if (level != 170 || !Game1.player.hasQuest(ModEntry.QiChallengeFinalQuestId)) return true; // run original logic

            ___netIsTreasureRoom.Value = true;
            __instance.loadedMapNumber = 120;
            __instance.mapPath.Value = "Maps\\Mines\\10";
            __instance.updateMap();
            __instance.ApplyDiggableTileFixes();
            MineShaft.lowestLevelReached = Math.Max(MineShaft.lowestLevelReached, level);
            return false; // don't run original logic
        }
    }

    [HarmonyPatch(typeof(MineShaft), "addLevelChests")]
    internal class MineShaftAddLevelChestsPatch
    {
        /// <summary>Add custom Qi Challenge reward.</summary>
        [HarmonyPostfix]
        private static void Postfix(MineShaft __instance)
        {
            if (__instance.mineLevel != 170 || Game1.player.hasOrWillReceiveMail("QiChallengeComplete") ||
                !Game1.player.hasQuest(ModEntry.QiChallengeFinalQuestId)) return;

            var chestSpot = new Vector2(9f, 9f);
            __instance.overlayObjects[chestSpot] =
                new Chest(0, new() {new SObject(Constants.GALAXY_SOUL_INDEX_I, 1)}, chestSpot)
                {
                    Tint = Color.White
                };
        }
    }

    [HarmonyPatch(typeof(BaseEnchantment), nameof(BaseEnchantment.GetAvailableEnchantments))]
    internal class BaseEnchantmentGetAvailableEnchantmentsPatch
    {
        /// <summary>Allow applying magic/sunburst enchant.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var l = instructions.ToList();
            l.InsertRange(l.Count - 2, new List<CodeInstruction>
            {
                new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                new(OpCodes.Newobj, typeof(MagicEnchantment).RequireConstructor()),
                new(OpCodes.Callvirt, typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add)))
            });

            return l.AsEnumerable();
        }
    }

    #endregion harmony patches
}
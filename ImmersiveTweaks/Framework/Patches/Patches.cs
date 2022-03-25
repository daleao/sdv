namespace DaLion.Stardew.Tweaks.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Patches the game code to implement modded tweak behavior.</summary>
[UsedImplicitly]
internal static class Patches
{
    #region harmony patches

    [HarmonyPatch(typeof(Bush), "shake")]
    internal class BushShakePatch
    {
        /// <summary>Detects if the bush is ready for harvest.</summary>
        [HarmonyPrefix]
        // ReSharper disable once RedundantAssignment
        private static bool BushShakePrefix(Bush __instance, ref bool __state)
        {
            __state = __instance is not null && __instance.tileSheetOffset.Value == 1 && !__instance.townBush.Value &&
                      __instance.inBloom(Game1.GetSeasonForLocation(__instance.currentLocation), Game1.dayOfMonth) &&
                      __instance.size.Value < Bush.greenTeaBush && ModEntry.Config.BerryBushesRewardExp;

            return true; // run original logic
        }

        /// <summary>Adds foraging experience if the bush was harvested.</summary>
        [HarmonyPostfix]
        private static void BushShakePostfix(Bush __instance, bool __state)
        {
            if (__state && __instance.tileSheetOffset.Value == 0)
                Game1.player.gainExperience((int) SkillType.Foraging, 3);
        }
    }

    [HarmonyPatch(typeof(FruitTree), nameof(FruitTree.dayUpdate))]
    internal class FruitTreeDayUpdatePatch
    {
        /// <summary>Negatively compensates winter growth.</summary>
        [HarmonyPostfix]
        private static void FruitTreeDayUpdatePostfix(FruitTree __instance)
        {
            if (__instance.growthStage.Value < FruitTree.treeStage && Game1.IsWinter &&
                ModEntry.Config.PreventFruitTreeGrowthInWinter)
                ++__instance.daysUntilMature.Value;
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.checkForAction))]
    internal class ObjectCheckForActionPatch
    {
        /// <summary>Detects if a tapper is ready for harvest.</summary>
        [HarmonyPrefix]
        // ReSharper disable once RedundantAssignment
        private static bool ObjectCheckForActionPrefix(SObject __instance, ref bool __state)
        {
            __state = __instance.name.Contains("Tapper") && __instance.heldObject.Value is not null &&
                      __instance.readyForHarvest.Value && ModEntry.Config.TappersRewardExp;
            return true; // run original logic
        }

        /// <summary>Adds foraging experience if a tapper was harvested.</summary>
        [HarmonyPostfix]
        private static void ObjectCheckForActionPostfix(SObject __instance, bool __state)
        {
            if (__state && !__instance.readyForHarvest.Value)
                Game1.player.gainExperience((int)SkillType.Foraging, 5);
        }

        /// <summary>Applies quality to aged bee house.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ObjectCheckForActionTranspiler(
            IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// Injected: heldObject.Value.Quality = this.GetQualityFromAge();
            /// After: heldObject.Value.preservedParentSheetIndex.Value = honey_type;

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldstr, " Honey")
                    )
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldfld, typeof(SObject).Field(nameof(SObject.preservedParentSheetIndex)))
                    )
                    .RetreatUntil(
                        new CodeInstruction(OpCodes.Ldarg_0)
                    )
                    .GetInstructionsUntil(out var got, false, true,
                        new CodeInstruction(OpCodes.Callvirt)
                    )
                    .AdvanceUntil(
                        new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.currentLocation)))
                    )
                    .Insert(got)
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call,
                            typeof(SObjectExtensions).MethodNamed(nameof(SObjectExtensions.GetQualityFromAge))),
                        new CodeInstruction(OpCodes.Callvirt, typeof(SObject).PropertySetter(nameof(SObject.Quality)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed while improving honey quality with age.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.DayUpdate))]
    internal class ObjectDayUpdatePatch
    {
        /// <summary>Age bee houses.</summary>
        [HarmonyPostfix]
        private static void ObjectDayUpdatePostfix(SObject __instance)
        {
            if (__instance.IsBeeHouse() && ModEntry.Config.AgeBeeHouses) __instance.IncrementData<int>("Age");
        }
    }

    [HarmonyPatch(typeof(SObject), "loadDisplayName")]
    internal class ObjectLoadDisplayNamePatch
    {
        /// <summary>Add flower-specific mead names.</summary>
        [HarmonyPostfix]
        private static void ObjectLoadDisplayNamePostfix(SObject __instance, ref string __result)
        {
            if (!__instance.name.Contains("Mead") || __instance.preservedParentSheetIndex.Value <= 0 ||
                !ModEntry.Config.KegsRememberHoneyFlower) return;

            var prefix = Game1.objectInformation[__instance.preservedParentSheetIndex.Value].Split('/')[4];
            __result = prefix + ' ' + __result;
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.performObjectDropInAction))]
    internal class ObjectPerformObjectDropInActionPatch
    {
        // <summary>Remember state before action.</summary>
        [HarmonyPrefix]
        // ReSharper disable once RedundantAssignment
        private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
        {
            __state = __instance.heldObject.Value !=
                      null; // remember whether this machine was already holding an object
            return true; // run original logic
        }

        /// <summary>Tweaks golden and ostrich egg artisan products + gives flower memory to kegs.</summary>
        [HarmonyPostfix]
        private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, bool __state, Item dropInItem,
            bool probe, Farmer who)
        {
            // if there was an object inside before running the original method, or if the machine is still empty after running the original method, then do nothing
            if (__state || __instance.heldObject.Value is null || probe || dropInItem is not SObject dropIn) return;

            // kegs remember honey flower type
            if (__instance.name == "Keg" && dropIn.ParentSheetIndex == 340 &&
                dropIn.preservedParentSheetIndex.Value > 0 && ModEntry.Config.KegsRememberHoneyFlower)
            {
                __instance.heldObject.Value.name = dropIn.name.Split(" Honey")[0] + " Mead";
                __instance.heldObject.Value.preservedParentSheetIndex.Value =
                    dropIn.preservedParentSheetIndex.Value;
                __instance.heldObject.Value.Price = dropIn.Price * 2;
            }
            // large milk/eggs give double output at normal quality
            else if (dropInItem.Name.ContainsAnyOf("Large", "L.") && ModEntry.Config.LargeProducsYieldQuantityOverQuality)
            {
                __instance.heldObject.Value.Stack = 2;
                __instance.heldObject.Value.Quality = SObject.lowQuality;
            }
            else if (ModEntry.Config.LargeProducsYieldQuantityOverQuality) switch (dropInItem.ParentSheetIndex)
            {
                // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
                case 289 when !ModEntry.ModHelper.ModRegistry.IsLoaded("ughitsmegan.ostrichmayoForProducerFrameworkMod"):
                    __instance.heldObject.Value.Quality = SObject.lowQuality;
                    break;
                // golden mayonnaise keeps giving gives single output but keeps golden quality
                case 928 when !ModEntry.ModHelper.ModRegistry.IsLoaded("ughitsmegan.goldenmayoForProducerFrameworkMod"):
                    __instance.heldObject.Value.Stack = 1;
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(Tree), nameof(Tree.dayUpdate))]
    internal class TreeDayUpdatePatch
    {
        /// <summary>Ages tapper trees.</summary>
        [HarmonyPostfix]
        private static void TreeDayUpdatePostfix(Tree __instance, int __state)
        {
            if (__instance.growthStage.Value >= Tree.treeStage && __instance.CanBeTapped() &&
                ModEntry.Config.AgeTapperTrees) __instance.IncrementData<int>("Age");
        }
    }

    [HarmonyPatch(typeof(Tree), nameof(Tree.UpdateTapperProduct))]
    internal class TreeUpdateTapperProductPatch
    {
        /// <summary>Adds age quality to tapper product.</summary>
        [HarmonyPostfix]
        private static void TreeUpdateTapperProductPostfix(Tree __instance, SObject tapper_instance)
        {
            if (tapper_instance is not null)
                tapper_instance.heldObject.Value.Quality = __instance.GetQualityFromAge();
        }
    }

    #endregion harmony patches
}
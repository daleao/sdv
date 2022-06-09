namespace DaLion.Stardew.Tweaks.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Classes;
using Common.Extensions;
using Common.Extensions.Reflection;
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
        private static bool Prefix(Bush __instance, ref bool __state)
        {
            __state = __instance is not null && __instance.tileSheetOffset.Value == 1 && !__instance.townBush.Value &&
                      __instance.inBloom(Game1.GetSeasonForLocation(__instance.currentLocation), Game1.dayOfMonth) &&
                      __instance.size.Value < Bush.greenTeaBush && ModEntry.Config.BerryBushesRewardExp;

            return true; // run original logic
        }

        /// <summary>Adds foraging experience if the bush was harvested.</summary>
        [HarmonyPostfix]
        private static void Postfix(Bush __instance, bool __state)
        {
            if (__state && __instance.tileSheetOffset.Value == 0)
                Game1.player.gainExperience((int) SkillType.Foraging, 3);
        }
    }

    [HarmonyPatch(typeof(Crop), nameof(Crop.hitWithHoe))]
    internal class CropHitWithHoePatch
    {
        /// <summary>Apply Botanist/Ecologist perk to wild ginger.</summary>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// Injected: SetGingerQuality(@object);
            /// Between: @object = new SObject(829, 1);

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Stloc_0)
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Call,
                            typeof(CropHitWithHoePatch).RequireMethod(nameof(AddGingerQuality)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed while apply Ecologist/Botanist perk to hoed ginger.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }

        private static SObject AddGingerQuality(SObject ginger)
        {
            if (!ModEntry.Config.ProfessionalForagingInGingerIsland || !Game1.player.professions.Contains(Farmer.botanist)) return ginger;

            ginger.Quality = ModEntry.ProfessionsAPI is null
                ? SObject.bestQuality
                : ModEntry.ProfessionsAPI.GetForageQuality(Game1.player);
            return ginger;
        }
    }

    [HarmonyPatch(typeof(FruitTree), nameof(FruitTree.dayUpdate))]
    internal class FruitTreeDayUpdatePatch
    {
        /// <summary>Negatively compensates winter growth.</summary>
        [HarmonyPostfix]
        private static void Postfix(FruitTree __instance)
        {
            if (__instance.growthStage.Value < FruitTree.treeStage && Game1.IsWinter &&
                !__instance.currentLocation.IsGreenhouse && ModEntry.Config.PreventFruitTreeGrowthInWinter)
                ++__instance.daysUntilMature.Value;
        }
    }

    [HarmonyPatch(typeof(FruitTree), nameof(FruitTree.shake))]
    internal class FruitTreeshakePatch
    {
        /// <summary>Customize Fruit Tree age quality.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("aedenthorn.FruitTreeTweaks")) return instructions;
            
            var helper = new ILHelper(original, instructions);

            /// From: int fruitQuality = 0;
            /// To: int fruitQuality = this.GetQualityFromAge();
            /// Removed all remaining age checks for quality
            
            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc_0)
                    )
                    .StripLabels(out var labels)
                    .ReplaceWith(new(OpCodes.Call, typeof(FruitTreeExtensions).RequireMethod(nameof(FruitTreeExtensions.GetQualityFromAge))))
                    .InsertWithLabels(
                        labels,
                        new CodeInstruction(OpCodes.Ldarg_0)
                    )
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldarg_0)
                    )
                    .RemoveUntil(
                        new CodeInstruction(OpCodes.Stloc_0)
                    )
                    .RemoveUntil(
                        new CodeInstruction(OpCodes.Stloc_0)
                    )
                    .RemoveUntil(
                        new CodeInstruction(OpCodes.Stloc_0)
                    )
                    .RemoveLabels();
            }
            catch (Exception ex)
            {
                Log.E($"Failed customizing fruit tree age quality factor.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.explode))]
    internal class GameLocationExplodePatch
    {
        /// <summary>Explosions trigger nearby bombs.</summary>
        [HarmonyPostfix]
        private static void Postfix(GameLocation __instance, Vector2 tileLocation, int radius)
        {
            if (!ModEntry.Config.ExplosionTriggeredBombs) return;

            var circle = new CircleTileGrid(tileLocation, radius * 2);
            foreach (var sprite in __instance.TemporarySprites.Where(sprite => sprite.bombRadius > 0 && circle.Tiles.Contains(sprite.Position / 64f)))
                sprite.currentNumberOfLoops = sprite.totalNumberOfLoops;
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.checkForAction))]
    internal class ObjectCheckForActionPatch
    {
        /// <summary>Detects if an object is ready for harvest.</summary>
        [HarmonyPrefix]
        // ReSharper disable once RedundantAssignment
        private static bool Prefix(SObject __instance, ref bool __state)
        {
            __state = __instance.heldObject.Value is not null &&
                      __instance.readyForHarvest.Value;
            return true; // run original logic
        }

        /// <summary>Adds foraging experience if a tapper or mushroom box was harvested.</summary>
        [HarmonyPostfix]
        private static void Postfix(SObject __instance, bool __state)
        {
            if (!__state || __instance.readyForHarvest.Value) return;
            
            if (__instance.name.Contains("Tapper") && ModEntry.Config.TappersRewardExp)
                Game1.player.gainExperience((int) SkillType.Foraging, 5);
            else if (__instance.name.Contains("Mushroom Box") && ModEntry.Config.MushroomBoxesRewardExp)
                Game1.player.gainExperience((int) SkillType.Foraging, 1);
        }

        /// <summary>Applies quality to aged bee house.</summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator, MethodBase original)
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
                        new CodeInstruction(OpCodes.Ldfld,
                            typeof(SObject).RequireField(nameof(SObject.preservedParentSheetIndex)))
                    )
                    .RetreatUntil(
                        new CodeInstruction(OpCodes.Ldarg_0)
                    )
                    .GetInstructionsUntil(out var got, false, true,
                        new CodeInstruction(OpCodes.Callvirt)
                    )
                    .AdvanceUntil(
                        new CodeInstruction(OpCodes.Call,
                            typeof(Game1).RequirePropertyGetter(nameof(Game1.currentLocation)))
                    )
                    .Insert(got)
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call,
                            typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.GetQualityFromAge))),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(SObject).RequirePropertySetter(nameof(SObject.Quality)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed improving honey quality with age.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.DayUpdate))]
    internal class ObjectDayUpdatePatch
    {
        /// <summary>Age bee houses and mushroom boxes.</summary>
        [HarmonyPostfix]
        [HarmonyPriority(Priority.LowerThanNormal)]
        private static void Postfix(SObject __instance)
        {
            if (__instance.IsBeeHouse() && ModEntry.Config.AgeBeeHouses)
            {
                __instance.IncrementData<int>("Age");
            }
            else if (__instance.IsMushroomBox())
            {
                if (ModEntry.Config.AgeMushroomBoxes) __instance.IncrementData<int>("Age");

                if (__instance.heldObject.Value is null) return;

                __instance.heldObject.Value.Quality = ModEntry.ProfessionsAPI is null
                    ? __instance.GetQualityFromAge()
                    : Math.Max(ModEntry.ProfessionsAPI.GetForageQuality(Game1.player),
                        __instance.heldObject.Value.Quality);
            }
        }
    }

    [HarmonyPatch(typeof(SObject), "loadDisplayName")]
    internal class ObjectLoadDisplayNamePatch
    {
        /// <summary>Add flower-specific mead names.</summary>
        [HarmonyPostfix]
        private static void Postfix(SObject __instance, ref string __result)
        {
            if (!__instance.name.Contains("Mead") || __instance.preservedParentSheetIndex.Value <= 0 ||
                !ModEntry.Config.KegsRememberHoneyFlower) return;

            var prefix = Game1.objectInformation[__instance.preservedParentSheetIndex.Value].Split('/')[4];
            __result = prefix + ' ' + __result;
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.performDropDownAction))]
    internal class ObjectPerformDropDownActionPatch
    {
        /// <summary>Clear the age of bee houses and mushroom boxes.</summary>
        [HarmonyPostfix]
        private static void Postfix(SObject __instance)
        {
            if (__instance.IsBeeHouse() || __instance.IsMushroomBox())
                __instance.WriteData("Age", null);
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.performObjectDropInAction))]
    internal class ObjectPerformObjectDropInActionPatch
    {
        // <summary>Remember state before action.</summary>
        [HarmonyPrefix]
        // ReSharper disable once RedundantAssignment
        private static bool Prefix(SObject __instance, ref bool __state)
        {
            __state = __instance.heldObject.Value !=
                      null; // remember whether this machine was already holding an object
            return true; // run original logic
        }

        /// <summary>Tweaks golden and ostrich egg artisan products + gives flower memory to kegs.</summary>
        [HarmonyPostfix]
        private static void Postfix(SObject __instance, bool __state, Item dropInItem,
            bool probe)
        {
            // if there was an object inside before running the original method, or if the machine is still empty after running the original method, then do nothing
            if (__state || __instance.heldObject.Value is null || probe || dropInItem is not SObject dropIn) return;

            // kegs remember honey flower type
            if (__instance.name == "Keg" && dropIn.ParentSheetIndex == 340 &&
                dropIn.preservedParentSheetIndex.Value > 0 && ModEntry.Config.KegsRememberHoneyFlower)
            {
                __instance.heldObject.Value.name = dropIn.name.Split(" Honey")[0] + " Mead";
                __instance.heldObject.Value.honeyType.Value = (SObject.HoneyType) dropIn.preservedParentSheetIndex.Value;
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
        private static void Postfix(Tree __instance, int __state)
        {
            if (__instance.growthStage.Value >= Tree.treeStage && __instance.CanBeTapped() &&
                ModEntry.Config.AgeSapTrees) __instance.IncrementData<int>("Age");
        }
    }

    [HarmonyPatch(typeof(Tree), nameof(Tree.UpdateTapperProduct))]
    internal class TreeUpdateTapperProductPatch
    {
        /// <summary>Adds age quality to tapper product.</summary>
        [HarmonyPostfix]
        private static void Postfix(Tree __instance, SObject tapper_instance)
        {
            if (tapper_instance is not null)
                tapper_instance.heldObject.Value.Quality = __instance.GetQualityFromAge();
        }
    }

    [HarmonyPatch(typeof(Tree), "shake")]
    internal class TreeShakePatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            /// From: Game1.createObjectDebris(seedIndex, tileLocation.X, tileLocation.Y - 3, (tileLocation.Y + 1) * 64, 0, 1f, location);
            /// To: Game1.createObjectDebris(seedIndex, tileLocation.X, tileLocation.Y - 3, (tileLocation.Y + 1) * 64, GetCoconutQuality(), 1f, location);
            ///     -- and again for golden coconut immediately below

            try
            {
                var callCreateObjectDebrisInst = new CodeInstruction(OpCodes.Call,
                    typeof(Game1).RequireMethod(nameof(Game1.createObjectDebris),
                        new[]
                        {
                            typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float),
                            typeof(GameLocation)
                        }));

                helper
                    // the normal coconut
                    .FindFirst(callCreateObjectDebrisInst)
                    .RetreatUntil(
                        new CodeInstruction(OpCodes.Ldc_I4_0)
                    )
                    .ReplaceWith(
                        new CodeInstruction(OpCodes.Call,
                            typeof(TreeShakePatch).RequireMethod(nameof(GetCoconutQuality)))
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Ldloc_2)
                    )
                    // the golden coconut
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldc_I4, 791)
                    )
                    .AdvanceUntil(callCreateObjectDebrisInst)
                    .RetreatUntil(
                        new CodeInstruction(OpCodes.Ldc_I4_0)
                    )
                    .ReplaceWith(
                        new CodeInstruction(OpCodes.Call,
                            typeof(TreeShakePatch).RequireMethod(nameof(GetCoconutQuality)))
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Ldc_I4, 791)
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed applying Ecologist/Botanist perk to shaken coconut.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }

        private static int GetCoconutQuality(int seedIndex)
        {
            if (seedIndex is not (Constants.COCONUT_INDEX_I or Constants.GOLDEN_COCONUT_INDEX_I) ||
                !ModEntry.Config.ProfessionalForagingInGingerIsland ||
                !Game1.player.professions.Contains(Farmer.botanist))
                return SObject.lowQuality;

            return ModEntry.ProfessionsAPI is null
                ? SObject.bestQuality
                : ModEntry.ProfessionsAPI.GetForageQuality(Game1.player);
        }
    }

    #endregion harmony patches
}
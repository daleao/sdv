namespace DaLion.Stardew.Tweaks.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

internal static class AutomatePatches
{
    [CanBeNull] private static MethodInfo _GetBushMachine,
        _GetMushroomBoxMachine,
        _GetTapperMachine,
        _GetSampleFromCheesePressMachine,
        _GetSampleFromGenericMachine;

    internal static void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType().RequireMethod("OnOutputReduced"),
            postfix: new(typeof(AutomatePatches).RequireMethod(nameof(BushMachineOnOutputReducedPostfix)))
        );

        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.FruitTreeMachine".ToType().RequireMethod("GetOutput"),
            transpiler: new(typeof(AutomatePatches).RequireMethod(nameof(FruitTreeMachineGetOutputTranspiler)))
        );

        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine".ToType().RequireMethod("GetOutput"),
            transpiler: new(typeof(AutomatePatches).RequireMethod(nameof(BeeHouseMachineGetOutputTranspiler)))
        );

        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine".ToType().RequireMethod("SetInput"),
            transpiler: new(typeof(AutomatePatches).RequireMethod(nameof(CheesePressMachineSetInputTranspiler)))
        );

        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.GenericObjectMachine`1".ToType()
                .MakeGenericType(typeof(SObject))
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .FirstOrDefault(m => m.Name == "GenericPullRecipe" && m.GetParameters().Length == 3),
            transpiler: new(typeof(AutomatePatches).RequireMethod(nameof(GenericObjectMachineGenericPullRecipeTranspiler)))
        );

        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine".ToType().RequireMethod("Reset"),
            postfix: new(typeof(AutomatePatches), nameof(TapperMachineResetPostfix))
        );
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated berry bushes.</summary>
    private static void BushMachineOnOutputReducedPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.BerryBushesRewardExp) return;

        _GetBushMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
        var machine = (Bush) _GetBushMachine.Invoke(__instance, null);
        if (machine is null || machine.size.Value >= Bush.greenTeaBush) return;

        Game1.MasterPlayer.gainExperience(Farmer.foragingSkill, 3);
    }

    /// <summary>Adds custom aging quality to automated fruit tree.</summary>
    private static IEnumerable<CodeInstruction> FruitTreeMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("aedenthorn.FruitTreeTweaks")) return instructions;

        var helper = new ILHelper(original, instructions);

        /// From: int quality = 0;
        /// To: int quality = this.GetQualityFromAge();
        /// Removed all remaining age checks for quality
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_1)
                )
                .StripLabels(out var labels)
                .ReplaceWith(new(OpCodes.Call,
                    typeof(FruitTreeExtensions).RequireMethod(nameof(FruitTreeExtensions.GetQualityFromAge))))
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Stloc_1)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Stloc_1)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Stloc_1)
                )
                .RemoveLabels();
        }
        catch (Exception ex)
        {
            Log.E($"Failed customizing automated fruit tree age quality factor.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    /// <summary>Adds aging quality to automated bee houses.</summary>
    private static IEnumerable<CodeInstruction> BeeHouseMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: @object.Quality = @object.GetQualityFromAge();
        /// After: @object.preservedParentSheetIndex.Value = flowerId;

        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[4])
                )
                .Insert(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Call, typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.GetQualityFromAge))),
                    new CodeInstruction(OpCodes.Callvirt, typeof(SObject).RequirePropertySetter(nameof(SObject.Quality)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed improving automated honey quality with age.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    /// <summary>Replaces large milk output quality with quantity.</summary>
    private static IEnumerable<CodeInstruction> CheesePressMachineSetInputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: GenericPullRecipeSubroutine(this, consumable)
        /// Before: return true;

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call)
                )
                .GetInstructions(out var got, 2)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                )
                .Insert(got)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(AutomatePatches).RequireMethod(nameof(SetInputSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching automated Cheese Press.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    /// <summary>Replaces large egg output quality with quantity + add flower memory to automated kegs.</summary>
    private static IEnumerable<CodeInstruction> GenericObjectMachineGenericPullRecipeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: GenericPullRecipeSubroutine(this, consumable)
        /// Before: return true;

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call)
                )
                .GetInstructions(out var got, 2)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                )
                .Insert(got)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(AutomatePatches).RequireMethod(nameof(GenericPullRecipeSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Artisan behavior to generic Automate machines.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    /// <summary>Patch for automated Mushroom Box quality.</summary>
    private static void MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            if (__instance is null || !ModEntry.Config.AgeMushroomBoxes) return;

            _GetMushroomBoxMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
            var machine = (SObject) _GetMushroomBoxMachine.Invoke(__instance, null);
            if (machine?.heldObject.Value is not { } held) return;

            var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
            if (!owner.professions.Contains(Farmer.botanist))
                held.Quality = held.GetQualityFromAge();
            else if (ModEntry.ProfessionsAPI is not null)
                held.Quality = Math.Max(ModEntry.ProfessionsAPI.GetEcologistForageQuality(owner), held.Quality);
            else
                held.Quality = SObject.bestQuality;
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    /// <summary>Adds foraging experience for automated mushroom boxes.</summary>
    private static void MushroomBoxMachineGetOutputPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.MushroomBoxesRewardExp) return;

        _GetMushroomBoxMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
        var machine = (SObject) _GetMushroomBoxMachine.Invoke(__instance, null);
        if (machine is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        owner.gainExperience(Farmer.foragingSkill, 1);
    }

    /// <summary>Adds foraging experience for automated tappers.</summary>
    private static void TapperMachineResetPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.TappersRewardExp) return;

        _GetTapperMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
        var machine = (SObject) _GetTapperMachine.Invoke(__instance, null);
        if (machine is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        owner.gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches

    #region injected subroutines

    private static void SetInputSubroutine(SObject machine, object consumable)
    {
        if (!ModEntry.Config.LargeProducsYieldQuantityOverQuality) return;

        _GetSampleFromCheesePressMachine ??= consumable.GetType().RequirePropertyGetter("Sample");
        if (_GetSampleFromCheesePressMachine.Invoke(consumable, null) is not SObject input) return;

        var output = machine.heldObject.Value;
        if (!input.Name.ContainsAnyOf("Large", "L.")) return;

        output.Stack = 2;
        output.Quality = SObject.lowQuality;
    }

    private static void GenericPullRecipeSubroutine(SObject machine, object consumable)
    {
        if (machine.name != "Mayonnaise Machine" || machine.heldObject.Value is null ||
            !ModEntry.Config.LargeProducsYieldQuantityOverQuality) return;

        _GetSampleFromGenericMachine ??= consumable.GetType().RequirePropertyGetter("Sample");
        if (_GetSampleFromGenericMachine!.Invoke(consumable, null) is not SObject input) return;

        var output = machine.heldObject.Value;
        if (input.Name.ContainsAnyOf("Large", "L."))
        {
            output.Stack = 2;
            output.Quality = SObject.lowQuality;
        }
        else
        {
            switch (input.ParentSheetIndex)
            {
                // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
                case 289 when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                    "ughitsmegan.ostrichmayoForProducerFrameworkMod"):
                    output.Quality = SObject.lowQuality;
                    break;
                // golden mayonnaise keeps giving gives single output but keeps golden quality
                case 928 when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                    "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                    output.Stack = 1;
                    break;
            }
        }
    }

    #endregion injected subroutines
}
namespace DaLion.Stardew.Tweaks.Framework.Patches.Integrations;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Harmony;

using SObject = StardewValley.Object;

#endregion using directives

internal static class AutomatePatches
{
    private static MethodInfo _GetBushMachine, _GetTapperMachine, _GetSampleFromCheesePressMachine, _GetSampleFromGenericMachine;

    internal static void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType().RequireMethod("OnOutputReduced"),
            postfix: new(typeof(AutomatePatches).RequireMethod(nameof(BushMachineOnOutputReducedPostfix)))
        );

        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine".ToType().RequireMethod("SetInput"),
            transpiler: new(typeof(AutomatePatches).RequireMethod(nameof(CheesePressSetInputTranspiler)))
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

        Game1.MasterPlayer.gainExperience((int) SkillType.Foraging, 3);
    }

    /// <summary>Replaces large milk output quality with quantity.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CheesePressSetInputTranspiler(
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
    [HarmonyTranspiler]
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

    /// <summary>Adds foraging experience for automated tappers.</summary>
    [HarmonyPostfix]
    private static void TapperMachineResetPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.TappersRewardExp) return;

        _GetTapperMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
        var machine = (SObject)_GetTapperMachine.Invoke(__instance, null);
        if (machine is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        owner.gainExperience((int)SkillType.Foraging, 5);
    }

    #endregion harmony patches

    #region private methods

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
        if (machine.name != "Mayonnaise Machine" && machine.name != "Keg") return;

        _GetSampleFromGenericMachine ??= consumable.GetType().RequirePropertyGetter("Sample");
        if (_GetSampleFromGenericMachine.Invoke(consumable, null) is not SObject input) return;


        var output = machine.heldObject.Value;
        switch (machine.Name)
        {
            case "Mayonnaise Machine" when ModEntry.Config.LargeProducsYieldQuantityOverQuality:
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

                break;

            case "Keg" when input.ParentSheetIndex == 340 && input.preservedParentSheetIndex.Value > 0 && ModEntry.Config.KegsRememberHoneyFlower:
                output.name = input.name.Split(" Honey")[0] + " Mead";
                output.preservedParentSheetIndex.Value = input.preservedParentSheetIndex.Value;
                output.Price = input.Price * 2;
                break;
        }
    }

    #endregion private methods
}
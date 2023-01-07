namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class GenericObjectMachinePatchers : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GenericObjectMachinePatchers"/> class.</summary>
    internal GenericObjectMachinePatchers()
    {
        this.Transpiler!.before = new[] { OverhaulModule.Professions.Namespace };
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            base.ApplyImpl(harmony);
        }
    }

    /// <inheritdoc />
    protected override void UnapplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            base.UnapplyImpl(harmony);
        }
    }

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return "Pathoschild.Stardew.Automate.Framework.GenericObjectMachine`1".ToType()
            .MakeGenericType(typeof(SObject))
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .First(m => m.Name == "GenericPullRecipe" && m.GetParameters().Length == 3);

        yield return "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine".ToType()
            .RequireMethod("SetInput");
    }

    #region harmony patches

    /// <summary>Replaces large egg output quality with quantity + add flower memory to automated kegs.</summary>
    [HarmonyTranspiler]
    [HarmonyBefore("Overhaul.Modules.Professions")]
    private static IEnumerable<CodeInstruction>? GenericObjectMachineTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: GenericPullRecipeSubroutine(this, consumable)
        // Before: return true;
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ret),
                    },
                    ILHelper.SearchOption.Last)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            "Pathoschild.Stardew.Automate.Framework.BaseMachine`1"
                                .ToType()
                                .MakeGenericType(typeof(SObject))
                                .RequirePropertyGetter("Machine")),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            "Pathoschild.Stardew.Automate.IConsumable"
                                .ToType()
                                .RequirePropertyGetter("Sample")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GenericObjectMachinePatchers).RequireMethod(
                                original.DeclaringType!.Name.Contains("CheesePress")
                                    ? nameof(CheesePressMachineSubroutine)
                                    : nameof(GenericMachineSubroutine))),
                    });
        }
        catch (Exception ex)
        {
            Log.E("Immersive Tweaks Failed patching modded Artisan behavior to generic Automate machines." +
                  "\n—-- Do NOT report this to Automate's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void GenericMachineSubroutine(SObject machine, Item sample)
    {
        if (!TweexModule.Config.LargeProducsYieldQuantityOverQuality || machine.heldObject.Value is not { } output ||
            sample is not SObject input)
        {
            return;
        }

        if (input.Category is SObject.EggCategory or SObject.MilkCategory && input.Name.ContainsAnyOf("Large", "L."))
        {
            output.Stack = 2;
            output.Quality = SObject.lowQuality;
        }
        else if (machine.ParentSheetIndex == (int)Machine.MayonnaiseMachine)
        {
            switch (input.ParentSheetIndex)
            {
                // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
                case Constants.OstrichEggIndex when !ModHelper.ModRegistry.IsLoaded(
                    "ughitsmegan.ostrichmayoForProducerFrameworkMod"):
                    output.Quality = SObject.lowQuality;
                    break;
                // golden mayonnaise keeps giving gives single output but keeps golden quality
                case Constants.GoldenEggIndex when !ModHelper.ModRegistry.IsLoaded(
                    "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                    output.Stack = 1;
                    break;
            }
        }
    }

    private static void CheesePressMachineSubroutine(SObject machine, Item sample)
    {
        if (!TweexModule.Config.LargeProducsYieldQuantityOverQuality || machine.heldObject.Value is not { } output ||
            sample is not SObject input || !input.Name.ContainsAnyOf("Large", "L."))
        {
            return;
        }

        output.Stack = 2;
        output.Quality = SObject.lowQuality;
    }

    #endregion injected subroutines
}

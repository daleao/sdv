namespace DaLion.Redux.Tweex.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Tweex.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class FruitTreeMachineGetOutputPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeMachineGetOutputPatch"/> class.</summary>
    internal FruitTreeMachineGetOutputPatch()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.FruitTreeMachine"
            .ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Adds custom aging quality to automated fruit tree.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FruitTreeMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("aedenthorn.FruitTreeTweaks"))
        {
            return instructions;
        }

        var helper = new IlHelper(original, instructions);

        // From: int quality = 0;
        // To: int quality = this.GetQualityFromAge();
        // Removed all remaining age checks for quality
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_1))
                .StripLabels(out var labels)
                .ReplaceInstructionWith(new CodeInstruction(
                    OpCodes.Call,
                    typeof(FruitTreeExtensions).RequireMethod(nameof(FruitTreeExtensions.GetQualityFromAge))))
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_0))
                .FindNext(new CodeInstruction(OpCodes.Ldloc_0))
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Stloc_1))
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Stloc_1))
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Stloc_1))
                .RemoveLabels();
        }
        catch (Exception ex)
        {
            Log.E("Immersive Tweaks failed customizing automated fruit tree age quality factor." +
                  "\n—-- Do NOT report this to Automate's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}

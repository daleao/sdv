namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common;
using Common.Attributes;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly, RequiresMod("Pathoschild.Automate")]
internal sealed class FruitTreeMachineGetOutputPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FruitTreeMachineGetOutputPatch()
    {
        Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.FruitTreeMachine".ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Adds custom aging quality to automated fruit tree.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FruitTreeMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
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

    #endregion harmony patches
}
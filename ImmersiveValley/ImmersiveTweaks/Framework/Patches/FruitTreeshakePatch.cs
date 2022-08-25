namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeshakePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FruitTreeshakePatch()
    {
        Target = RequireMethod<FruitTree>(nameof(FruitTree.shake));
    }

    #region harmony patches

    /// <summary>Customize Fruit Tree age quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FruitTreeshakeTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
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
                .ReplaceInstructionWith(new(OpCodes.Call,
                    typeof(FruitTreeExtensions).RequireMethod(nameof(FruitTreeExtensions.GetQualityFromAge))))
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Stloc_0)
                )
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Stloc_0)
                )
                .RemoveInstructionsUntil(
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

    #endregion harmony patches
}
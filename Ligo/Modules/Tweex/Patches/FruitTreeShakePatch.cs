namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Tweex.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeShakePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeShakePatch"/> class.</summary>
    internal FruitTreeShakePatch()
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.shake));
    }

    #region harmony patches

    /// <summary>Customize Fruit Tree age quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FruitTreeShakeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("aedenthorn.FruitTreeTweaks"))
        {
            return instructions;
        }

        var helper = new IlHelper(original, instructions);

        // From: int fruitQuality = 0;
        // To: int fruitQuality = this.GetQualityFromAge();
        // Removed all remaining age checks for quality
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_0))
                .StripLabels(out var labels)
                .ReplaceInstructionWith(new CodeInstruction(
                    OpCodes.Call,
                    typeof(FruitTreeExtensions).RequireMethod(nameof(FruitTreeExtensions.GetQualityFromAge))))
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0))
                .FindNext(new CodeInstruction(OpCodes.Ldarg_0))
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Stloc_0))
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Stloc_0))
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Stloc_0))
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

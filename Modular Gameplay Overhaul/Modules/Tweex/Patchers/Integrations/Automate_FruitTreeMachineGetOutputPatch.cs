namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tweex.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class FruitTreeMachineGetOutputPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeMachineGetOutputPatcher"/> class.</summary>
    internal FruitTreeMachineGetOutputPatcher()
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
        if (ModHelper.ModRegistry.IsLoaded("aedenthorn.FruitTreeTweaks"))
        {
            return instructions;
        }

        var helper = new ILHelper(original, instructions);

        // From: int quality = 0;
        // To: int quality = this.GetQualityFromAge();
        // Removed all remaining age checks for quality
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc_1),
                    })
                .StripLabels(out var labels)
                .ReplaceWith(new CodeInstruction(
                    OpCodes.Call,
                    typeof(FruitTreeExtensions).RequireMethod(nameof(FruitTreeExtensions.GetQualityFromAge))))
                .Insert(
                    new[] { new CodeInstruction(OpCodes.Ldloc_0) },
                    labels)
                .Match(new[] { new CodeInstruction(OpCodes.Ldloc_0) })
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_1) }, out var count)
                .Remove(count)
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_1) }, out count)
                .Remove(count)
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_1) }, out count)
                .Remove(count)
                .StripLabels();
        }
        catch (Exception ex)
        {
            Log.E("Tweex module failed customizing automated fruit tree age quality factor." +
                  "\n—-- Do NOT report this to Automate's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}

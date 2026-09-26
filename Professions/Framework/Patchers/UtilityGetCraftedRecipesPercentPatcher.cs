namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityGetCraftedRecipesPercentPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityGetCraftedRecipesPercentPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal UtilityGetCraftedRecipesPercentPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.getCraftedRecipesPercent));
    }

    #region harmony patches

    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ObjectPlacementActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_3)])
                .PatternMatch([new CodeInstruction(OpCodes.Brtrue_S)])
                .GetOperand(out var continueLabel)
                .Return()
                .Move()
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldloc_3),
                        new CodeInstruction(OpCodes.Call, typeof(UtilityGetCraftedRecipesPercentPatcher).RequireMethod(nameof(ShouldSkip))),
                        new CodeInstruction(OpCodes.Brtrue_S, continueLabel)
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Arborist record for Trees.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool ShouldSkip(string key)
    {
        IReadOnlyList<string> exclusions = [
                "Slime Flute",
                "Red Paintbrush",
                "Green Paintbrush",
                "Blue Paintbrush",
                "Purple Paintbrush",
                "Prismatic Paintbrush",
                "Survey Flag",
                "Wild Bait Alt",
                "Deluxe Bait Alt",
                "Challenge Bait Alt",
                "Magic Bait Alt",
            ];
        return key.IsIn(exclusions);
    }

    #endregion injected
}

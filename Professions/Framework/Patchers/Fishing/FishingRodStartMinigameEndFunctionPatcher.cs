namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodStartMinigameEndFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodStartMinigameEndFunctionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishingRodStartMinigameEndFunctionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.startMinigameEndFunction));
    }

    #region harmony patches

    /// <summary>Patch to remove Pirate bonus treasure chance + double Fisher bait effect.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FishingRodStartMinigameEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...
        try
        {
            helper // find index of pirate check
                .MatchProfessionCheck(Farmer.pirate)
                .Move(-1)
                .RemoveUntil([new CodeInstruction(OpCodes.Add)]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var isNotFisher = generator.DefineLabel();
            var isNotPrestigedFisher = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[5])], ILHelper.SearchOption.First)
                .AddLabels(isNotFisher, isNotPrestigedFisher)
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_0),
                ])
                .InsertProfessionCheck(Farmer.fisher, forLocalPlayer: false)
                .Insert([new CodeInstruction(OpCodes.Brfalse_S, isNotFisher)])
                .Insert([
                    new CodeInstruction(OpCodes.Ldc_R8, 2d),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Ldloc_0),
                ])
            .InsertProfessionCheck(Farmer.fisher + 100, forLocalPlayer: false)
            .Insert([new CodeInstruction(OpCodes.Brfalse_S, isNotPrestigedFisher)])
            .Insert([
                new CodeInstruction(OpCodes.Ldc_R8, 2d),
                new CodeInstruction(OpCodes.Mul),
            ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed doubling Magnet effect for Fisher.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}

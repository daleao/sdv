namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationOnStoneDestroyedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationOnStoneDestroyedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationOnStoneDestroyedPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.OnStoneDestroyed));
    }

    #region harmony patches

    /// <summary>Patch to remove Prospector double coal chance.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationOnStoneDestroyedTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6])]) // burrowerMultiplier
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Pop),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Prospector double coal chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}

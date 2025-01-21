namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.UI;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class MasteryTrackerMenuClaimRewardPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MasteryTrackerMenuClaimRewardPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MasteryTrackerMenuClaimRewardPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MasteryTrackerMenu>("claimReward");
    }

    #region harmony patches

    /// <summary>Patch for post-Mastery unlocks.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MasteryTrackerMenuClaimRewardPostfix(int ___which)
    {
        if (___which == Skill.Combat && ShouldEnableLimitBreaks)
        {
            Game1.activeClickableMenu = new MasteryLimitSelectionPage();
        }
    }

    /// <summary>Patch to wait for Limit selection before spirit candles.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? MasteryTrackerMenuClaimRewardTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MasteryTrackerMenu).RequireMethod(
                            nameof(MasteryTrackerMenu.hasCompletedAllMasteryPlaques)))
                ])
                .Move()
                .GetOperand(out var dontTurnOnCandles)
                .Return()
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(MasteryTrackerMenu).RequireField("which")),
                    new CodeInstruction(OpCodes.Ldc_I4_4), // combat skill index
                    new CodeInstruction(OpCodes.Beq, dontTurnOnCandles),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed delaying spirit candles.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}

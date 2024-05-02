namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformOrePanTenMinuteUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformOrePanTenMinuteUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationPerformOrePanTenMinuteUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performOrePanTenMinuteUpdate));
    }

    #region harmony patches

    /// <summary>Patch to add bonus panning point spawn attempts to Prospector.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationPerformOrePanTenMinuteUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_8)])
                .Move()
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(GameLocationPerformOrePanTenMinuteUpdatePatcher).RequireMethod(nameof(GetBonusAttempts))),
                        new CodeInstruction(OpCodes.Add),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding bonus panning point spawn attempts to Prospectors.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static int GetBonusAttempts(GameLocation location)
    {
        if (!location.DoesAnyPlayerHereHaveProfession(Profession.Prospector, out var prospectors))
        {
            return 0;
        }

        var prospectorList = prospectors.ToList();
        return Math.Min(
            prospectorList.Aggregate(
                0,
                (current, prospector) => current + Data.ReadAs<int>(prospector, DataKeys.ScavengerHuntStreak)) / 2,
            prospectorList.Count * 8);
    }

    #endregion injections
}

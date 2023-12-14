namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformOrePanTenMinuteUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformOrePanTenMinuteUpdatePatcher"/> class.</summary>
    internal GameLocationPerformOrePanTenMinuteUpdatePatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performOrePanTenMinuteUpdate));
    }

    #region harmony patches

    /// <summary>Patch to add bonus panning point spawn attempts to Prospector.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationPerformOrePanTenMinuteUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_6) })
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(GameLocationPerformOrePanTenMinuteUpdatePatcher).RequireMethod(nameof(GetBonusAttempts))),
                        new CodeInstruction(OpCodes.Add),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding bonus panning point spawn attempts to Prospectors.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int GetBonusAttempts(GameLocation location)
    {
        return !location.DoesAnyPlayerHereHaveProfession(VanillaProfession.Prospector, out var prospectors)
            ? 0
            : Math.Min(
                prospectors.Aggregate(
                    0,
                    (current, prospector) => Math.Max(prospector.Read<int>(DataKeys.ScavengerHuntStreak), current)) / 2,
                100);
    }

    #endregion injected subroutines
}

namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondDoActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishPondDoActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.doAction));
    }

    #region harmony patches

    /// <summary>Insert check for Aquarist profession. Must be transpiled due to inlining.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FishPondDoActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Call, typeof(FishPond).RequireMethod("isLegalFishForPonds")),
                    ],
                    ILHelper.SearchOption.Last)
                .SetOperand(typeof(FishPondDoActionPatcher).RequireMethod(nameof(IsLegalFishForPonds)));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Aquarist legendary gate.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool IsLegalFishForPonds(FishPond pond, string itemId)
    {
        if (!ItemContextTagManager.HasBaseTag(itemId, "fish_legendary"))
        {
            return FishPond.GetRawData(itemId) is not null;
        }

        return $"(O){itemId}" is not (QIDs.SonOfCrimsonfish or QIDs.MsAngler or QIDs.LegendII or QIDs.RadioactiveCarp
            or QIDs.GlacierfishJr);
    }

    #endregion injected
}

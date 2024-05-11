namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondDoActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondDoActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.doAction));
    }

    private delegate void ShowObjectThrownIntoPondAnimationDelegate(
        FishPond instance, Farmer who, SObject whichObject, Action? callback = null);

    #region harmony patches

    /// <summary>Allow legendary fish to share a pond with their extended families.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishPondDoActionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (who.ActiveObject.ItemId != fishType)
        // To: if (!IsNotSameFishNorExtendedFamily(who.ActiveObject.ItemId, fishType))
        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.fishType))),
                    new CodeInstruction(OpCodes.Call, typeof(NetString).RequireMethod("op_Implicit")),
                    new CodeInstruction(OpCodes.Call, typeof(string).RequireMethod("op_Inequality")),
                    new CodeInstruction(OpCodes.Brfalse),
                ])
                .Move(2)
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishPondDoActionPatcher).RequireMethod(nameof(IsSameFishOrExtendedFamily))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding family ties to legendary fish in ponds.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool IsSameFishOrExtendedFamily(string heldId, string otherId)
    {
        return heldId != otherId && !(Lookups.FamilyPairs.TryGetValue($"(O){otherId}", out var pairId) && pairId == $"(O){heldId}");
    }

    #endregion injected subroutines
}

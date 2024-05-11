namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotPerformObjectDropInActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotPerformObjectDropInActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CrabPotPerformObjectDropInActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Fixes an issue when collecting trash while holding bait as Conservationist.</summary>
    [HarmonyPrefix]
    private static bool CrabPotPerformObjectDropInActionPrefix(CrabPot __instance, ref bool __result)
    {
        if (__instance.heldObject.Value is null)
        {
            return true; // run original logic;
        }

        __result = false;
        return false; // don't run original logic

    }

    /// <summary>Patch to allow Conservationist to place bait.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CrabPotPerformObjectDropInActionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: ... && (owner_farmer is null || !owner_farmer.professions.Contains(11)
        try
        {
            helper
                .MatchProfessionCheck(Farmer.mariner)
                .PatternMatch([new CodeInstruction(OpCodes.Ldloc_2)], ILHelper.SearchOption.Previous, nth: 2)
                .RemoveUntil([new CodeInstruction(OpCodes.Brtrue_S)]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing Conservationist bait restriction.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}

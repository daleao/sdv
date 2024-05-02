namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
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

    /// <summary>Patch to allow Luremaster to overbait.</summary>
    [HarmonyPostfix]
    private static void CrabPotPerformObjectDropInActionPostfix(CrabPot __instance, ref bool __result, Item dropInItem, bool probe, Farmer who)
    {
        if (probe || __instance.Location is not { } location || __instance.bait.Value is null || __result)
        {
            return;
        }

        if (dropInItem is not SObject { Category: SObject.baitCategory } bait ||
            !who.HasProfession(Profession.Luremaster))
        {
            return;
        }

        var overbait = Data.Read(__instance, DataKeys.Overbait).ParseList<string>();
        var capacity = who.HasProfession(Profession.Luremaster, true) ? 2 : 1;
        if (overbait.Count >= capacity)
        {
            return;
        }

        overbait.Add(bait.QualifiedItemId);
        Data.Write(__instance, DataKeys.Overbait, string.Join(',', overbait));
        location.playSound("Ship");
        __instance.lidFlapping = true;
        __instance.lidFlapTimer = 60f;
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
                .MatchProfessionCheck(Profession.Conservationist.Value)
                .PatternMatch([new CodeInstruction(OpCodes.Ldloc_1)], ILHelper.SearchOption.Previous)
                .PatternMatch([new CodeInstruction(OpCodes.Ldloc_1)], ILHelper.SearchOption.Previous)
                .CountUntil([new CodeInstruction(OpCodes.Brtrue_S)], out var count)
                .Remove(count);
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

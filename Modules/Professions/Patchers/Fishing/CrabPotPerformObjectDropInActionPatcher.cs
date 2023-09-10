namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotPerformObjectDropInActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotPerformObjectDropInActionPatcher"/> class.</summary>
    internal CrabPotPerformObjectDropInActionPatcher()
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.performObjectDropInAction));
    }

    #region harmony patches

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
                .Match(new[] { new CodeInstruction(OpCodes.Ldloc_1) }, ILHelper.SearchOption.Previous)
                .Match(new[] { new CodeInstruction(OpCodes.Ldloc_1) }, ILHelper.SearchOption.Previous)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Brtrue_S) }, out var count)
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

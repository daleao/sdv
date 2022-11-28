namespace DaLion.Ligo.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
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
                .FindProfessionCheck(Profession.Conservationist.Value)
                .RetreatUntil(new CodeInstruction(OpCodes.Ldloc_1))
                .RetreatUntil(new CodeInstruction(OpCodes.Ldloc_1))
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Brtrue_S));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing Conservationist bait restriction.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
